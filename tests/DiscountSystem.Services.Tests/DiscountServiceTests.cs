using DiscountSystem.Core.Interfaces;
using DiscountSystem.Core.Models;
using DiscountSystem.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace DiscountSystem.Services.Tests;

public class DiscountServiceTests
{
    private readonly Mock<IDiscountCodeRepository> _repositoryMock;
    private readonly Mock<ICodeGenerator> _codeGeneratorMock;
    private readonly IMemoryCache _cache;
    private readonly Mock<ILogger<DiscountService>> _loggerMock;
    private readonly DiscountService _sut;

    public DiscountServiceTests()
    {
        _repositoryMock = new Mock<IDiscountCodeRepository>();
        _codeGeneratorMock = new Mock<ICodeGenerator>();
        _cache = new MemoryCache(new MemoryCacheOptions());
        _loggerMock = new Mock<ILogger<DiscountService>>();
        
        _sut = new DiscountService(
            _repositoryMock.Object,
            _codeGeneratorMock.Object,
            _cache,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GenerateCodesAsync_WithValidCount_ShouldGenerateSuccessfully()
    {
        // Arrange
        const int count = 10;
        var ct = CancellationToken.None;
        var generatedCodes = new List<string>();
        for (var i = 0; i < count; i++)
        {
            generatedCodes.Add($"TEST{i:0000}");
        }

        _codeGeneratorMock.SetupSequence(x => x.GenerateCode())
            .Returns(generatedCodes[0])
            .Returns(generatedCodes[1])
            .Returns(generatedCodes[2])
            .Returns(generatedCodes[3])
            .Returns(generatedCodes[4])
            .Returns(generatedCodes[5])
            .Returns(generatedCodes[6])
            .Returns(generatedCodes[7])
            .Returns(generatedCodes[8])
            .Returns(generatedCodes[9]);

        _repositoryMock.Setup(x => x.GetExistingCodesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>());

        // Act
        var result = await _sut.GenerateCodesAsync(count, ct);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.GeneratedCount.Should().Be(count);
        result.RequestedCount.Should().Be(count);

        _repositoryMock.Verify(x => x.BulkInsertAsync(
            It.Is<IEnumerable<DiscountCode>>(codes => codes.Count() == count),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(2001)]
    public async Task GenerateCodesAsync_WithInvalidCount_ShouldThrowArgumentException(int count)
    {
        var ct = CancellationToken.None;
        // Act
        var act = () => _sut.GenerateCodesAsync(count, ct);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*between 1 and 2000*");
    }

    [Fact]
    public async Task UseCodeAsync_WithExistingUnusedCode_ShouldReturnTrue()
    {
        // Arrange
        const string code = "TESTCODE";
        var discountCode = new DiscountCode
        {
            Id = Guid.NewGuid(),
            Code = code,
            CreatedAt = DateTime.UtcNow,
            UsedAt = null
        };

        _repositoryMock.Setup(x => x.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(discountCode);

        // Act
        var result = await _sut.UseCodeAsync(code);

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(x => x.UpdateAsync(
            It.Is<DiscountCode>(dc => dc.UsedAt != null && dc.Code == code),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UseCodeAsync_WithNonExistentCode_ShouldReturnFalse()
    {
        // Arrange
        const string code = "BADCODE1";
        _repositoryMock.Setup(x => x.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DiscountCode?)null);

        // Act
        var result = await _sut.UseCodeAsync(code);

        // Assert
        result.Should().BeFalse();
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<DiscountCode>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UseCodeAsync_WithAlreadyUsedCode_ShouldReturnFalse()
    {
        // Arrange
        const string code = "USEDCODE";
        var discountCode = new DiscountCode
        {
            Id = Guid.NewGuid(),
            Code = code,
            CreatedAt = DateTime.UtcNow.AddHours(-1),
            UsedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(x => x.GetByCodeAsync(code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(discountCode);

        // Act
        var result = await _sut.UseCodeAsync(code);

        // Assert
        result.Should().BeFalse();
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<DiscountCode>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("SHORT")]
    [InlineData("TOOLONGCODE")]
    public async Task UseCodeAsync_WithInvalidFormat_ShouldReturnFalse(string code)
    {
        // Act
        var result = await _sut.UseCodeAsync(code);

        // Assert
        result.Should().BeFalse();
        _repositoryMock.Verify(x => x.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GenerateCodesAsync_ShouldHandleDuplicates()
    {
        // Arrange
        const int count = 5;
        var ct = CancellationToken.None;
        _codeGeneratorMock.SetupSequence(x => x.GenerateCode())
            .Returns("DUPE0001")
            .Returns("DUPE0001") // Duplicate
            .Returns("UNIQ0002")
            .Returns("UNIQ0003")
            .Returns("UNIQ0004")
            .Returns("UNIQ0005");

        _repositoryMock.Setup(x => x.GetExistingCodesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HashSet<string>());

        // Act
        var result = await _sut.GenerateCodesAsync(count,ct);

        // Assert
        result.Success.Should().BeTrue();
        result.GeneratedCount.Should().Be(count);
        _codeGeneratorMock.Verify(x => x.GenerateCode(), Times.Exactly(6)); // One extra due to duplicate
    }
}
using FluentAssertions;

namespace DiscountSystem.Services.Tests;

public class CodeGeneratorTests
{
    private readonly CodeGenerator _sut = new();

    [Fact]
    public void GenerateCode_ShouldReturn8Characters()
    {
        // Act
        var code = _sut.GenerateCode();

        // Assert
        code.Should().NotBeNullOrEmpty();
        code.Length.Should().Be(8);
    }

    [Fact]
    public void GenerateCode_ShouldOnlyContainAlphanumericCharacters()
    {
        // Act
        var code = _sut.GenerateCode();

        // Assert
        code.Should().MatchRegex("^[A-Z0-9]{8}$");
    }

    [Fact]
    public void GenerateCode_ShouldGenerateUniqueCodesVeryHighProbability()
    {
        // Arrange
        var codes = new HashSet<string>();
        const int iterations = 10000;

        // Act
        for (var i = 0; i < iterations; i++)
        {
            codes.Add(_sut.GenerateCode());
        }

        // Assert
        codes.Count.Should().Be(iterations, "all generated codes should be unique");
    }

    [Fact]
    public async Task GenerateCode_ShouldBeThreadSafe()
    {
        // Arrange
        var codes = new System.Collections.Concurrent.ConcurrentBag<string>();
        var tasks = new List<Task>();

        // Act
        for (var i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (var j = 0; j < 100; j++)
                {
                    codes.Add(_sut.GenerateCode());
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        codes.Should().HaveCount(1000);
        codes.Distinct().Should().HaveCount(1000, "all codes should be unique even when generated concurrently");
    }
}
using DiscountSystem.API;
using FluentAssertions;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace DiscountSystem.API.Tests;

public class GrpcIntegrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task GenerateCodes_WithValidRequest_ShouldReturnSuccess()
    {
        // Arrange
        var client = CreateClient();
        var request = new GenerateRequest { Count = 10 };

        // Act
        var response = await client.GenerateCodesAsync(request);

        // Assert
        response.Result.Should().BeTrue();
    }

    [Fact]
    public async Task UseCode_WithNonExistentCode_ShouldReturnFailure()
    {
        // Arrange
        var client = CreateClient();
        var request = new UseCodeRequest { Code = "INVALID1" };

        // Act
        var response = await client.UseCodeAsync(request);

        // Assert
        response.Result.Should().Be(1); // Failure
    }

    private Discount.DiscountClient CreateClient()
    {
        var httpClient = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
        }).CreateClient();

        var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = httpClient
        });

        return new Discount.DiscountClient(channel);
    }
}

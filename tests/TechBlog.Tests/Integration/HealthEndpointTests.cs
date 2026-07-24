using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TechBlog.Tests.Integration;

public sealed class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReturnsResponse()
    {
        var response = await _client.GetAsync("/health");
        Assert.NotNull(response);
        Assert.True(
            response.StatusCode is System.Net.HttpStatusCode.OK or System.Net.HttpStatusCode.ServiceUnavailable,
            $"Expected 200 or 503, got {(int)response.StatusCode}");
    }

    [Fact]
    public async Task Health_ReturnsJsonOrText()
    {
        var response = await _client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrWhiteSpace(content));
    }
}

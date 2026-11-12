using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using TechBlog.Infrastructure.Persistence;
using TechBlog.Tests.Configuration;
using Xunit;

namespace TechBlog.Tests.Integration;

public sealed class HealthTestFactory : WebApplicationFactory<Program>
{
    private readonly DisposableTestDatabase _database = DisposableTestDatabase.FromEnvironment();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Set test values before Program validates configuration.
        builder.ConfigureHostConfiguration(config => config.AddInMemoryCollection(_database.Configuration));
        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(_database.Configuration));
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DataContext>();
            services.RemoveAll<DbContextOptions<DataContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<DataContext>>();
            services.AddDbContext<DataContext>(options => options.UseNpgsql(_database.Connection));
        });
    }
}

public sealed class HealthEndpointTests : IClassFixture<HealthTestFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(HealthTestFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });
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

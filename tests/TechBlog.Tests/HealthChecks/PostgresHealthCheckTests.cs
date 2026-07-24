using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using TechBlog.Infrastructure.HealthChecks;
using Xunit;

namespace TechBlog.Tests.HealthChecks;

public sealed class PostgresHealthCheckTests
{
    [Fact]
    public void ImplementsIHealthCheck()
    {
        var healthCheckType = typeof(PostgresHealthCheck);
        Assert.True(typeof(IHealthCheck).IsAssignableFrom(healthCheckType));
    }

    [Fact]
    public void Constructor_WithConfiguration_Succeeds()
    {
        var config = new ConfigurationBuilder().Build();
        var sut = new PostgresHealthCheck(config);
        Assert.NotNull(sut);
    }

    [Fact]
    public async Task CheckHealthAsync_MissingConnectionString_ReturnsUnhealthy()
    {
        var config = new ConfigurationBuilder().Build();
        var sut = new PostgresHealthCheck(config);

        var result = await sut.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Contains("not configured", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_InvalidHost_ReturnsUnhealthy()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Postgres_Db"] = "Host=nonexistent.invalid;Port=5432;Database=test;Username=test;Password=test;Timeout=2;CommandTimeout=2"
            })
            .Build();
        var sut = new PostgresHealthCheck(config);

        var result = await sut.CheckHealthAsync(new HealthCheckContext());

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.NotNull(result.Exception);
    }
}

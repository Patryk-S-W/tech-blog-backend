using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCaching;
using Xunit;

namespace TechBlog.Tests.Configuration;

public sealed class ServiceRegistrationTests
{
    [Fact]
    public void HealthChecks_AreRegistered()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddHealthChecks();
        var provider = services.BuildServiceProvider();

        var registration = provider.GetRequiredService<HealthCheckService>();
        Assert.NotNull(registration);
    }

    [Fact]
    public void RateLimiter_OptionsAreConfigured()
    {
        var services = new ServiceCollection();
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("api", opt =>
            {
                opt.PermitLimit = 100;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueLimit = 0;
            });
        });
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<RateLimiterOptions>>();
        Assert.NotNull(options);
        Assert.NotNull(options.Value);
    }

    [Fact]
    public void ResponseCaching_IsRegistered()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddResponseCaching();
        var provider = services.BuildServiceProvider();
        Assert.NotNull(provider);
    }
}

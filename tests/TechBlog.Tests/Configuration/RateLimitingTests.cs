using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Xunit;

namespace TechBlog.Tests.Configuration;

public sealed class RateLimitingTests
{
    private static readonly Assembly WebApiAssembly = typeof(Program).Assembly;

    [Theory]
    [InlineData("AnnouncementController")]
    [InlineData("ProjectController")]
    public void AdminControllers_HaveEnableRateLimiting(string controllerName)
    {
        var controllerType = WebApiAssembly.GetTypes()
            .First(t => t.Name == controllerName);

        var attr = controllerType.GetCustomAttribute<EnableRateLimitingAttribute>();
        Assert.NotNull(attr);
        Assert.Equal("api", attr.PolicyName);
    }

    [Theory]
    [InlineData("BlogController")]
    [InlineData("BlogProjectController")]
    [InlineData("AuthController")]
    public void PublicControllers_DoNotHaveEnableRateLimiting(string controllerName)
    {
        var controllerType = WebApiAssembly.GetTypes()
            .First(t => t.Name == controllerName);

        var attr = controllerType.GetCustomAttribute<EnableRateLimitingAttribute>();
        Assert.Null(attr);
    }
}

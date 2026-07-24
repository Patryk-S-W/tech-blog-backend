using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace TechBlog.Tests.Configuration;

public sealed class ApiVersioningTests
{
    private static readonly Assembly WebApiAssembly = typeof(Program).Assembly;

    [Fact]
    public void AllControllers_HaveApiVersionAttribute()
    {
        var controllerTypes = WebApiAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ControllerBase).IsAssignableFrom(t))
            .ToList();

        foreach (var controllerType in controllerTypes)
        {
            var hasVersionAttr = controllerType.GetCustomAttribute<ApiVersionAttribute>() is not null;
            var isNeutral = controllerType.GetCustomAttribute<ApiVersionNeutralAttribute>() is not null;
            Assert.True(
                hasVersionAttr || isNeutral,
                $"{controllerType.Name} is missing [ApiVersion] or [ApiVersionNeutral]");
        }
    }

    [Fact]
    public void VersionedControllers_HaveCorrectRouteTemplate()
    {
        var controllerTypes = WebApiAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ControllerBase).IsAssignableFrom(t))
            .Where(t => t.GetCustomAttribute<ApiVersionNeutralAttribute>() is null)
            .ToList();

        foreach (var controllerType in controllerTypes)
        {
            var routeAttr = controllerType.GetCustomAttribute<RouteAttribute>();
            Assert.NotNull(routeAttr);
            Assert.Contains("v{version:apiVersion}", routeAttr.Template);
        }
    }
}

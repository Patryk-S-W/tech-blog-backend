using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechBlog.WebApi.Controllers;
using Xunit;

namespace TechBlog.Tests.Configuration;

public sealed class AuthEndpointTests
{
    [Fact]
    public void PublicAuthEndpoints_ExposeOnlyLoginAndRefresh()
    {
        Assert.NotNull(typeof(AuthController).GetCustomAttribute<AllowAnonymousAttribute>());
        var endpoints = typeof(AuthController).GetMethods()
            .SelectMany(method => method.GetCustomAttributes<HttpPostAttribute>())
            .Select(attribute => attribute.Template)
            .OrderBy(template => template)
            .ToArray();

        Assert.Equal(new[] { "login", "refresh" }, endpoints);
        Assert.Null(typeof(AuthController).GetMethod("Register"));
    }
}

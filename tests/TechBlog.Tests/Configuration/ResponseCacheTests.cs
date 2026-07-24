using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace TechBlog.Tests.Configuration;

public sealed class ResponseCacheTests
{
    private static readonly Assembly WebApiAssembly = typeof(Program).Assembly;

    [Fact]
    public void BlogController_GetPublished_HasResponseCache()
    {
        var controllerType = WebApiAssembly.GetType("TechBlog.WebApi.Controllers.BlogController")!;
        var method = controllerType.GetMethod("GetPublished")!;
        var attr = method.GetCustomAttribute<ResponseCacheAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(30, attr.Duration);
        Assert.Contains("page", attr.VaryByQueryKeys!);
        Assert.Contains("pageSize", attr.VaryByQueryKeys!);
    }

    [Fact]
    public void BlogController_GetPublishedById_HasResponseCache()
    {
        var controllerType = WebApiAssembly.GetType("TechBlog.WebApi.Controllers.BlogController")!;
        var method = controllerType.GetMethod("GetPublishedById")!;
        var attr = method.GetCustomAttribute<ResponseCacheAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(60, attr.Duration);
    }

    [Theory]
    [InlineData("BlogProjectController", "GetAll")]
    [InlineData("BlogProjectController", "GetById")]
    public void BlogProjectEndpoints_HaveResponseCache(string controllerName, string methodName)
    {
        var controllerType = WebApiAssembly.GetType($"TechBlog.WebApi.Controllers.{controllerName}")!;
        var method = controllerType.GetMethod(methodName)!;
        var attr = method.GetCustomAttribute<ResponseCacheAttribute>();
        Assert.NotNull(attr);
        Assert.True(attr.Duration > 0);
    }

    [Theory]
    [InlineData("AnnouncementController", "GetAll")]
    [InlineData("ProjectController", "GetAll")]
    public void AdminControllers_DoNotHaveResponseCache(string controllerName, string methodName)
    {
        var controllerType = WebApiAssembly.GetType($"TechBlog.WebApi.Controllers.{controllerName}")!;
        var method = controllerType.GetMethod(methodName)!;
        var attr = method.GetCustomAttribute<ResponseCacheAttribute>();
        Assert.Null(attr);
    }
}

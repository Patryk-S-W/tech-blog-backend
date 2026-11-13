using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TechBlog.WebApi.Middleware;
using Xunit;

namespace TechBlog.Tests.Configuration;

public sealed class ConcurrencyResponseTests
{
    [Fact]
    public async Task ConcurrencyFailure_ReturnsSanitizedConflict()
    {
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new DbUpdateConcurrencyException("private-database-sentinel"),
            NullLogger<ExceptionHandlingMiddleware>.Instance);
        var context = new DefaultHttpContext();
        using var body = new MemoryStream();
        context.Response.Body = body;
        await middleware.InvokeAsync(context);
        Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        body.Position = 0;
        var response = await new StreamReader(body).ReadToEndAsync();
        Assert.Contains("Reload", response);
        Assert.DoesNotContain("private-database-sentinel", response);
    }
}

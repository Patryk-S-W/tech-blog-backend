using Xunit;

namespace TechBlog.Tests.Configuration;

public sealed class DisposableTestDatabaseTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("private-sentinel")]
    [InlineData("Host=localhost;Database=production;Password=private-sentinel")]
    [InlineData("Host=remote.invalid;Database=techblog_test_case;Password=private-sentinel")]
    public void UnsafeConnection_IsRejectedWithoutEcho(string? connection)
    {
        var error = Assert.Throws<InvalidOperationException>(() => DisposableTestDatabase.ValidateConnection(connection));
        Assert.DoesNotContain("private-sentinel", error.Message);
        Assert.Null(error.InnerException);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("short-private-sentinel")]
    public void MissingOrShortTestKey_IsRejected(string? key)
    {
        var error = Assert.Throws<InvalidOperationException>(() => DisposableTestDatabase.Validate(
            "Host=localhost;Database=techblog_test_guard", key));
        Assert.DoesNotContain("private-sentinel", error.Message);
        Assert.Null(error.InnerException);
    }
}

using Microsoft.Extensions.Configuration;
using TechBlog.WebApi.Configuration;
using Xunit;

namespace TechBlog.Tests.Configuration;

public sealed class StartupConfigurationTests
{
    private const string Connection = "Host=localhost;Database=techblog_test;Username=test;Password=test-only";

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("Host=localhost;Password=${POSTGRES_PASSWORD}")]
    [InlineData("not-a-connection-string")]
    [InlineData("Password=private-value")]
    [InlineData("Host=localhost;Port=not-a-number;Password=private-value")]
    public void InvalidConnection_IsRejectedWithoutLeakingValues(string? connection)
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            StartupConfiguration.Validate(Config(connection, new string('k', 64))));

        Assert.Equal("ConnectionStrings:Postgres_Db must contain a valid PostgreSQL connection string.", exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("${JWT_SECRET}")]
    [InlineData("short-private-key")]
    public void InvalidToken_IsRejectedWithoutLeakingValues(string? token)
    {
        var exception = Assert.Throws<InvalidOperationException>(() =>
            StartupConfiguration.Validate(Config(Connection, token)));

        Assert.Equal("AppSettings:Token must contain a signing key of at least 64 UTF-8 bytes.", exception.Message);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Token_Requires64Utf8Bytes()
    {
        Assert.Throws<InvalidOperationException>(() =>
            StartupConfiguration.Validate(Config(Connection, new string('k', 63))));
        StartupConfiguration.Validate(Config(Connection, new string('k', 64)));
        StartupConfiguration.Validate(Config(Connection, new string('é', 32)));
        Assert.Throws<InvalidOperationException>(() =>
            StartupConfiguration.Validate(Config(Connection, "${JWT_SECRET}" + new string('k', 64))));
    }

    private static IConfiguration Config(string? connection, string? token) =>
        new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Postgres_Db"] = connection,
            ["AppSettings:Token"] = token,
        }).Build();
}

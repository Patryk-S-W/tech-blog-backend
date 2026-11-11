using System.Text;
using Npgsql;

namespace TechBlog.Tests.Configuration;

public sealed record DisposableTestDatabase(string Connection, string JwtKey)
{
    public static DisposableTestDatabase FromEnvironment() => Validate(
        Environment.GetEnvironmentVariable("TECHBLOG_TEST_CONNECTION"),
        Environment.GetEnvironmentVariable("TECHBLOG_TEST_JWT_KEY"));

    public static string ValidateConnection(string? connection)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(connection)) throw new ArgumentException();
            var settings = new NpgsqlConnectionStringBuilder(connection);
            if (settings.Host is not ("localhost" or "127.0.0.1")
                || settings.Database is null
                || !settings.Database.StartsWith("techblog_test_", StringComparison.Ordinal))
                throw new ArgumentException();
            return connection;
        }
        catch (Exception ex) when (ex is ArgumentException or FormatException or OverflowException)
        {
            throw new InvalidOperationException("TECHBLOG_TEST_CONNECTION must target a loopback techblog_test_ database.");
        }
    }

    public static DisposableTestDatabase Validate(string? connection, string? jwtKey)
    {
        var validated = ValidateConnection(connection);
        if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Contains("${") || Encoding.UTF8.GetByteCount(jwtKey) < 64)
            throw new InvalidOperationException("TECHBLOG_TEST_JWT_KEY must contain at least 64 UTF-8 bytes.");
        return new(validated, jwtKey);
    }

    public Dictionary<string, string?> Configuration => new()
    {
        ["ConnectionStrings:Postgres_Db"] = Connection,
        ["AppSettings:Token"] = JwtKey,
        ["AppSettings:Issuer"] = "TechBlogTests",
        ["AppSettings:Audience"] = "TechBlogTests",
    };
}

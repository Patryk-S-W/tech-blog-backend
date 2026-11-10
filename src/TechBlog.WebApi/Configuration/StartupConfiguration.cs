using System.Text;
using Npgsql;

namespace TechBlog.WebApi.Configuration;

public static class StartupConfiguration
{
    public static void Validate(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres_Db");
        if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("${"))
            throw new InvalidOperationException("ConnectionStrings:Postgres_Db must contain a valid PostgreSQL connection string.");

        try
        {
            var settings = new NpgsqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(settings.Host))
                throw new ArgumentException();
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException)
        {
            // Do not include the parser exception: it may contain connection credentials.
            throw new InvalidOperationException("ConnectionStrings:Postgres_Db must contain a valid PostgreSQL connection string.");
        }

        var token = configuration["AppSettings:Token"];
        if (string.IsNullOrWhiteSpace(token) || token.Contains("${") || Encoding.UTF8.GetByteCount(token) < 64)
            throw new InvalidOperationException("AppSettings:Token must contain a signing key of at least 64 UTF-8 bytes.");
    }
}

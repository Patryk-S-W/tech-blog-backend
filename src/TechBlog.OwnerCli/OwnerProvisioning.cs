using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TechBlog.Application;
using TechBlog.Application.Auth.Commands.Register;
using TechBlog.Domain.Common;
using TechBlog.Domain.Users;
using TechBlog.Infrastructure;
using TechBlog.Infrastructure.Persistence;

namespace TechBlog.OwnerCli;

public static class OwnerProvisioning
{
    public static async Task<int> RunAsync(string[] args, TextReader input, TextWriter output,
        Func<string> readHiddenPassword, IConfiguration configuration, CancellationToken ct = default)
    {
        string? username = null;
        var stdin = false;
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--username" && username is null && i + 1 < args.Length
                && !args[i + 1].StartsWith("--")) username = args[++i];
            else if (args[i] == "--password-stdin" && !stdin) stdin = true;
            else return await Fail(output, "Invalid arguments. Use --username NAME [--password-stdin].", 2);
        }
        if (string.IsNullOrWhiteSpace(username))
            return await Fail(output, "A --username is required.", 2);

        try
        {
            ct.ThrowIfCancellationRequested();
            var connection = configuration.GetConnectionString("Postgres_Db");
            try
            {
                if (string.IsNullOrWhiteSpace(connection) || connection.Contains("${"))
                    throw new ArgumentException();
                var settings = new NpgsqlConnectionStringBuilder(connection);
                if (string.IsNullOrWhiteSpace(settings.Host)) throw new ArgumentException();
            }
            catch (Exception ex) when (ex is ArgumentException or FormatException or OverflowException)
            {
                return await Fail(output, "ConnectionStrings:Postgres_Db must be a valid PostgreSQL connection string.", 2);
            }

            string password;
            if (stdin)
            {
                password = await input.ReadLineAsync(ct) ?? string.Empty;
            }
            else
            {
                await output.WriteAsync("Password: ");
                password = readHiddenPassword();
                await output.WriteAsync("Confirm password: ");
                if (password != readHiddenPassword())
                    return await Fail(output, "Passwords do not match.", 2);
            }

            var command = new RegisterCommand(username, password);
            var validation = await new RegisterCommandValidator().ValidateAsync(command, ct);
            if (!validation.IsValid)
                return await Fail(output, "Username or password does not meet registration policy.", 2);

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddApplication();
            services.AddInfrastructure(configuration);
            await using var provider = services.BuildServiceProvider();
            await using var scope = provider.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            // Provisioning never upgrades a database as a side effect.
            if (!(await db.Database.GetAppliedMigrationsAsync(ct)).Any()
                || (await db.Database.GetPendingMigrationsAsync(ct)).Any())
                return await Fail(output, "Database schema must be migrated before provisioning.", 1);

            var handler = new RegisterCommandHandler(
                scope.ServiceProvider.GetRequiredService<IUserRepository>(),
                scope.ServiceProvider.GetRequiredService<IPasswordHasher>(),
                scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
            var id = await handler.Handle(command, ct);
            await output.WriteLineAsync($"Account created (id {id}).");
            return 0;
        }
        catch (OperationCanceledException)
        {
            return await Fail(output, "Provisioning cancelled.", 1);
        }
        catch (DomainException)
        {
            return await Fail(output, "Account could not be created; check username and existing accounts.", 1);
        }
        catch (Exception)
        {
            // Database/provider errors may contain connection credentials.
            return await Fail(output, "Provisioning failed. Check database configuration, schema and connectivity.", 1);
        }
    }

    private static async Task<int> Fail(TextWriter output, string message, int code)
    {
        await output.WriteLineAsync(message);
        return code;
    }
}

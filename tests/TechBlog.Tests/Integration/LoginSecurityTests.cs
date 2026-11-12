using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechBlog.Infrastructure.Persistence;
using TechBlog.Infrastructure.Services;
using TechBlog.Domain.Users;
using Xunit;

namespace TechBlog.Tests.Integration;

// The test host runs migrations, so it must use a disposable database.
public sealed class LoginSecurityTests
{
    [Fact]
    public async Task FailedPasswords_PersistAcrossRequests_AndLockAccount()
    {
        await using var factory = new HealthTestFactory();
        using var client = factory.CreateClient(new()
        {
            BaseAddress = new Uri("https://localhost"),
        });
        var username = "lock-" + Guid.NewGuid().ToString("N");
        const string password = "Test-only-Password-123!";
        await SeedUser(factory, username, password);

        for (var attempt = 1; attempt <= 5; attempt++)
        {
            using var response = await client.PostAsJsonAsync("/api/auth/login", new
            {
                username,
                password = "incorrect-password",
            });
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            await using var scope = factory.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            var user = await db.Users.SingleAsync(u => u.Username == Username.Create(username));
            Assert.Equal(attempt, user.FailedLoginAttempts);
            Assert.Equal(attempt == 5, user.IsLockedOut);
        }

        using var lockedResponse = await client.PostAsJsonAsync("/api/auth/login", new { username, password });
        Assert.Equal(HttpStatusCode.Unauthorized, lockedResponse.StatusCode);
        await using var finalScope = factory.Services.CreateAsyncScope();
        var finalDb = finalScope.ServiceProvider.GetRequiredService<DataContext>();
        var lockedUser = await finalDb.Users.SingleAsync(u => u.Username == Username.Create(username));
        Assert.Equal(5, lockedUser.FailedLoginAttempts);
        Assert.True(lockedUser.IsLockedOut);
    }

    [Fact]
    public async Task SuccessfulLogin_BeforeLockout_PersistsResetAndReturnsTokens()
    {
        await using var factory = new HealthTestFactory();
        using var client = factory.CreateClient(new()
        {
            BaseAddress = new Uri("https://localhost"),
        });
        var username = "reset-" + Guid.NewGuid().ToString("N");
        const string password = "Test-only-Password-123!";
        await SeedUser(factory, username, password);

        using var failure = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username,
            password = "incorrect-password",
        });
        Assert.Equal(HttpStatusCode.Unauthorized, failure.StatusCode);
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            var user = await db.Users.SingleAsync(u => u.Username == Username.Create(username));
            Assert.Equal(1, user.FailedLoginAttempts);
        }

        using var success = await client.PostAsJsonAsync("/api/auth/login", new { username, password });
        Assert.Equal(HttpStatusCode.OK, success.StatusCode);
        var tokens = await success.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(tokens);
        Assert.Equal(2, tokens.Count);
        Assert.False(string.IsNullOrWhiteSpace(tokens["accessToken"]));
        Assert.False(string.IsNullOrWhiteSpace(tokens["refreshToken"]));
        client.DefaultRequestHeaders.Authorization = new("Bearer", tokens["accessToken"]);
        using var protectedResponse = await client.GetAsync("/api/v1/announcement");
        Assert.Equal(HttpStatusCode.OK, protectedResponse.StatusCode);

        await using var finalScope = factory.Services.CreateAsyncScope();
        var finalDb = finalScope.ServiceProvider.GetRequiredService<DataContext>();
        var resetUser = await finalDb.Users.SingleAsync(u => u.Username == Username.Create(username));
        Assert.Equal(0, resetUser.FailedLoginAttempts);
        Assert.Null(resetUser.LockoutEnd);
    }

    private static async Task SeedUser(HealthTestFactory factory, string username, string password)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        db.Users.Add(User.Register(Username.Create(username), new BCryptPasswordHasher().Hash(password)));
        await db.SaveChangesAsync();
    }
}

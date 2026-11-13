using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TechBlog.Domain.Users;
using TechBlog.Infrastructure.Persistence;
using TechBlog.Infrastructure.Services;
using TechBlog.OwnerCli;
using TechBlog.Tests.Configuration;
using Xunit;

namespace TechBlog.Tests.Integration;

public sealed class OwnerProvisioningTests
{
    private static IConfiguration InputConfiguration() => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Postgres_Db"] = "Host=localhost;Database=techblog_test_input",
        }).Build();
    [Fact]
    public async Task CancelledInput_ReturnsSanitizedFailure()
    {
        var output = new StringWriter();
        var code = await OwnerProvisioning.RunAsync(["--username", "cancel-test"],
            new StringReader(""), output,
            () => throw new OperationCanceledException("private-sentinel"), InputConfiguration());
        Assert.Equal(1, code);
        Assert.Contains("Provisioning cancelled.", output.ToString());
        Assert.DoesNotContain("private-sentinel", output.ToString());
    }

    [Fact]
    public async Task ConfirmationMismatch_RejectsWithoutEchoingPasswords()
    {
        var output = new StringWriter();
        var values = new Queue<string>(["private-first", "private-second"]);
        var code = await OwnerProvisioning.RunAsync(["--username", "confirmation-test"],
            new StringReader(""), output, () => values.Dequeue(), InputConfiguration());
        Assert.Equal(2, code);
        Assert.Contains("Passwords do not match.", output.ToString());
        Assert.DoesNotContain("private-first", output.ToString());
        Assert.DoesNotContain("private-second", output.ToString());
    }

    [Theory]
    [InlineData("--password")]
    [InlineData("--unknown")]
    public async Task UnsupportedArguments_AreNotEchoed(string option)
    {
        var output = new StringWriter();
        var code = await OwnerProvisioning.RunAsync(
            ["--username", "test", option, "private-sentinel"],
            new StringReader(""), output, () => throw new InvalidOperationException(), InputConfiguration());
        Assert.Equal(2, code);
        Assert.DoesNotContain("private-sentinel", output.ToString());
    }

    [Fact]
    public async Task StdinCreation_EnforcesPolicy_AndDuplicateDoesNotOverwrite()
    {
        // Schema setup belongs to the test, never the provisioning command.
        var database = DisposableTestDatabase.FromEnvironment();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(database.Configuration).Build();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseNpgsql(configuration.GetConnectionString("Postgres_Db")).Options;
        await using (var setup = new DataContext(options))
            await setup.Database.MigrateAsync();
        var username = "owner-" + Guid.NewGuid().ToString("N");
        const string password = "Test-Only-Password-123!";
        var output = new StringWriter();
        Task<int> Run(string value) => OwnerProvisioning.RunAsync(
            ["--username", username, "--password-stdin"], new StringReader(value + "\n"),
            output, () => throw new InvalidOperationException(), configuration);

        Assert.Equal(2, await Run("weak"));
        await using (var db = new DataContext(options))
            Assert.False(await db.Users.AnyAsync(u => u.Username == Username.Create(username)));
        Assert.Equal(0, await Run(password));
        string originalHash;
        int originalId;
        await using (var db = new DataContext(options))
        {
            var user = await db.Users.SingleAsync(u => u.Username == Username.Create(username));
            originalHash = user.PasswordHash;
            originalId = user.Id;
            Assert.NotEqual(password, originalHash);
            Assert.True(new BCryptPasswordHasher().Verify(password, originalHash));
        }
        Assert.Equal(1, await Run("Different-Password-123!"));
        await using (var db = new DataContext(options))
        {
            var user = await db.Users.SingleAsync(u => u.Username == Username.Create(username));
            Assert.Equal(originalHash, user.PasswordHash);
            Assert.Equal(originalId, user.Id);
        }
        Assert.DoesNotContain(password, output.ToString());
        Assert.DoesNotContain(originalHash, output.ToString());
        Assert.DoesNotContain("Different-Password-123!", output.ToString());
    }
}

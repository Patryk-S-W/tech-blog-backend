using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using TechBlog.Infrastructure.Persistence;
using TechBlog.Tests.Configuration;
using Xunit;

namespace TechBlog.Tests.Integration;

public sealed class AnnouncementPublicationMigrationTests
{
    [Fact]
    public async Task Upgrade_PreservesLegacyStateAndDates_AndDefaultsNewRowsToDraft()
    {
        var settings = DisposableTestDatabase.FromEnvironment();
        var name = "techblog_test_upgrade_" + Guid.NewGuid().ToString("N");
        var ownedSettings = new NpgsqlConnectionStringBuilder(settings.Connection) { Database = name, Pooling = false };
        var ownedConnection = DisposableTestDatabase.ValidateConnection(ownedSettings.ConnectionString);
        await using var admin = new NpgsqlConnection(settings.Connection);
        await admin.OpenAsync();
        var created = false;
        try
        {
            await using (var create = new NpgsqlCommand($"CREATE DATABASE \"{name}\"", admin))
                await create.ExecuteNonQueryAsync();
            created = true;
            var options = new DbContextOptionsBuilder<DataContext>().UseNpgsql(ownedConnection).Options;
            await using (var old = new DataContext(options))
                await old.GetService<IMigrator>().MigrateAsync("20260719091321_AddSecurityFieldsAndRefreshTokens");

            var knownDate = new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            int ownerId;
            await using (var connection = new NpgsqlConnection(ownedConnection))
            {
                await connection.OpenAsync();
                await using (var owner = new NpgsqlCommand("""
                    INSERT INTO "Users" ("Username", "PasswordHash", "Active", "DateModified", "IsDeleted")
                    VALUES ('migration-owner', 'test-only-unused-hash', true, @date, false) RETURNING "Id";
                    """, connection))
                {
                    owner.Parameters.AddWithValue("date", knownDate);
                    ownerId = (int)(await owner.ExecuteScalarAsync())!;
                }
                await using var seed = new NpgsqlCommand("""
                    INSERT INTO "Announcements"
                    ("Title", "Slug", "Image", "Text", "Category", "Duration", "Button", "UserId",
                     "Active", "DateCreated", "DateModified", "IsDeleted")
                    VALUES
                    ('Known', 'known', 'legacy-image', 'legacy-content', 'legacy', '1', 'Read', @owner, true, @date, @date, false),
                    ('Unknown', 'unknown', 'legacy-image', 'legacy-content', 'legacy', '1', 'Read', @owner, true, NULL, @date, false),
                    ('Inactive', 'inactive', 'legacy-image', 'legacy-content', 'legacy', '1', 'Read', @owner, false, @date, @date, false);
                    """, connection);
                seed.Parameters.AddWithValue("owner", ownerId);
                seed.Parameters.AddWithValue("date", knownDate);
                await seed.ExecuteNonQueryAsync();
            }

            await using (var upgraded = new DataContext(options))
            {
                await upgraded.Database.MigrateAsync();
                Assert.False(upgraded.Database.HasPendingModelChanges());
                var rows = await upgraded.Announcements.OrderBy(a => a.Slug).ToListAsync();
                Assert.Equal(3, rows.Count);
                foreach (var row in rows)
                {
                    Assert.Equal(ownerId, row.UserId);
                    Assert.Equal("legacy-content", row.Text);
                    Assert.Equal("legacy-image", row.Image);
                    Assert.Equal("legacy", row.Category);
                    Assert.Equal(knownDate, row.DateModified);
                }
                var known = rows.Single(a => a.Slug == "known");
                Assert.True(known.Active);
                Assert.Equal(knownDate, known.DateCreated);
                Assert.Equal(knownDate, known.PublishedAt);
                var unknown = rows.Single(a => a.Slug == "unknown");
                Assert.True(unknown.Active);
                Assert.Null(unknown.DateCreated);
                Assert.Null(unknown.PublishedAt);
                var inactive = rows.Single(a => a.Slug == "inactive");
                Assert.False(inactive.Active);
                Assert.Equal(knownDate, inactive.DateCreated);
                Assert.Null(inactive.PublishedAt);
                Assert.Equal(2, await upgraded.Announcements.CountAsync(a => a.Active));
            }

            await using var checkConnection = new NpgsqlConnection(ownedConnection);
            await checkConnection.OpenAsync();
            await using var insert = new NpgsqlCommand("""
                INSERT INTO "Announcements"
                ("Title", "Slug", "Image", "Text", "Category", "Duration", "Button", "UserId", "DateModified", "IsDeleted")
                VALUES ('Default draft', 'default-draft', '', 'Test', '', '', '', @owner, @date, false)
                RETURNING "Active", "PublishedAt";
                """, checkConnection);
            insert.Parameters.AddWithValue("owner", ownerId);
            insert.Parameters.AddWithValue("date", knownDate);
            await using var reader = await insert.ExecuteReaderAsync();
            Assert.True(await reader.ReadAsync());
            Assert.False(reader.GetBoolean(0));
            Assert.True(reader.IsDBNull(1));
        }
        finally
        {
            // Only this test's generated database is ever dropped.
            if (created)
            {
                await using var drop = new NpgsqlCommand($"DROP DATABASE \"{name}\" WITH (FORCE)", admin);
                await drop.ExecuteNonQueryAsync();
            }
        }
    }
}

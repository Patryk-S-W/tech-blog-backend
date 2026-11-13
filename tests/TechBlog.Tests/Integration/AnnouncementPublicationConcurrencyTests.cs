using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechBlog.Domain.Announcements;
using TechBlog.Domain.Users;
using TechBlog.Infrastructure.Persistence;
using Xunit;

namespace TechBlog.Tests.Integration;

public sealed class AnnouncementPublicationConcurrencyTests
{
    [Fact]
    public async Task StaleDraft_AfterPublishAndUnpublish_CannotReplaceFirstPublication()
    {
        await using var factory = new HealthTestFactory();
        using var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        int id;
        var title = "aba-" + Guid.NewGuid().ToString("N");
        await using (var seedScope = factory.Services.CreateAsyncScope())
        {
            var db = seedScope.ServiceProvider.GetRequiredService<DataContext>();
            var owner = User.Register(Username.Create(title), "test-only-unused-hash");
            db.Users.Add(owner);
            await db.SaveChangesAsync();
            var draft = Announcement.Create(title, "", "Original content", "", "", "", owner.Id);
            db.Announcements.Add(draft);
            await db.SaveChangesAsync();
            id = draft.Id;
        }

        await using var staleScope = factory.Services.CreateAsyncScope();
        await using var publishingScope = factory.Services.CreateAsyncScope();
        var staleDb = staleScope.ServiceProvider.GetRequiredService<DataContext>();
        var publishingDb = publishingScope.ServiceProvider.GetRequiredService<DataContext>();
        var stale = await staleDb.Announcements.SingleAsync(a => a.Id == id);
        var current = await publishingDb.Announcements.SingleAsync(a => a.Id == id);
        Assert.False(stale.Active);
        Assert.Null(stale.PublishedAt);

        current.Publish("publisher");
        await publishingDb.SaveChangesAsync();
        var firstPublication = current.PublishedAt;
        Assert.NotNull(firstPublication);
        current.Unpublish("publisher");
        await publishingDb.SaveChangesAsync();

        stale.Publish("stale");
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => staleDb.SaveChangesAsync());

        await using var freshScope = factory.Services.CreateAsyncScope();
        var freshDb = freshScope.ServiceProvider.GetRequiredService<DataContext>();
        var saved = await freshDb.Announcements.SingleAsync(a => a.Id == id);
        Assert.False(saved.Active);
        Assert.Equal(firstPublication, saved.PublishedAt);
        Assert.Equal("Original content", saved.Text);
        Assert.False(saved.IsDeleted);
        Assert.Equal("publisher", saved.LastModifiedBy);
    }

    [Theory]
    [InlineData("publish")]
    [InlineData("update")]
    [InlineData("delete")]
    public async Task StaleMutation_CannotOverwriteFirstPublication(string mutation)
    {
        await using var factory = new HealthTestFactory();
        using var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        int id;
        var title = "concurrency-" + Guid.NewGuid().ToString("N");
        await using (var seedScope = factory.Services.CreateAsyncScope())
        {
            var db = seedScope.ServiceProvider.GetRequiredService<DataContext>();
            var owner = User.Register(Username.Create(title), "test-only-unused-hash");
            db.Users.Add(owner);
            await db.SaveChangesAsync();
            var draft = Announcement.Create(title, "", "Original content", "", "", "", owner.Id);
            db.Announcements.Add(draft);
            await db.SaveChangesAsync();
            id = draft.Id;
        }

        await using var firstScope = factory.Services.CreateAsyncScope();
        await using var staleScope = factory.Services.CreateAsyncScope();
        var firstDb = firstScope.ServiceProvider.GetRequiredService<DataContext>();
        var staleDb = staleScope.ServiceProvider.GetRequiredService<DataContext>();
        var first = await firstDb.Announcements.SingleAsync(a => a.Id == id);
        var stale = await staleDb.Announcements.SingleAsync(a => a.Id == id);
        Assert.False(first.Active);
        Assert.False(stale.Active);

        first.Publish("first");
        await firstDb.SaveChangesAsync();
        var firstPublication = first.PublishedAt;
        Assert.NotNull(firstPublication);

        switch (mutation)
        {
            case "publish": stale.Publish("stale"); break;
            case "update": stale.Update(title, "", "Stale content", "", "", "", "stale"); break;
            case "delete": stale.MarkDeleted("stale"); break;
        }
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => staleDb.SaveChangesAsync());

        await using var freshScope = factory.Services.CreateAsyncScope();
        var freshDb = freshScope.ServiceProvider.GetRequiredService<DataContext>();
        var saved = await freshDb.Announcements.SingleAsync(a => a.Id == id);
        Assert.True(saved.Active);
        Assert.Equal(firstPublication, saved.PublishedAt);
        Assert.Equal("Original content", saved.Text);
        Assert.False(saved.IsDeleted);
        Assert.Equal("first", saved.LastModifiedBy);
    }
}

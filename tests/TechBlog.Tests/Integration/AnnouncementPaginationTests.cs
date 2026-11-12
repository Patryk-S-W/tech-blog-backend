using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechBlog.Application.Announcements;
using TechBlog.Application.Common.Pagination;
using TechBlog.Domain.Announcements;
using TechBlog.Domain.Users;
using TechBlog.Infrastructure.Persistence;
using Xunit;

namespace TechBlog.Tests.Integration;

public sealed class AnnouncementPaginationTests
{
    [Fact]
    public async Task FilteredPages_CountOnlyPublicRows_AndOrderTiesByDescendingId()
    {
        await using var factory = new HealthTestFactory();
        using var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        var category = "pagination-" + Guid.NewGuid().ToString("N");
        var visibleIds = new List<int>();
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            var owner = User.Register(Username.Create(category), "test-only-unused-hash");
            db.Users.Add(owner);
            await db.SaveChangesAsync();
            for (var i = 0; i < 9; i++)
            {
                var rowCategory = i == 7 ? category + "-draft" : i == 8 ? category + "-deleted" : category;
                var article = Announcement.Create(category + "-" + i, "", "Test content", rowCategory, "", "", owner.Id);
                if (i != 5 && i != 7) article.Publish("test");
                if (i == 6 || i == 8) article.MarkDeleted("test");
                db.Announcements.Add(article);
                // Tie timestamps so ordering cannot accidentally depend on insert timing.
                db.Entry(article).Property(a => a.DateCreated).CurrentValue =
                    new DateTime(2090, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                await db.SaveChangesAsync();
                if (i < 5) visibleIds.Add(article.Id);
            }
        }
        visibleIds.Reverse();
        var filter = "&category=" + Uri.EscapeDataString(category);
        var first = await Page(client, "?page=1&pageSize=2" + filter);
        AssertPage(first, 1, 2, 5, 3, false, true);
        Assert.Equal(visibleIds.Take(2), first.Items.Select(a => a.Id));
        var second = await Page(client, "?page=2&pageSize=2" + filter);
        AssertPage(second, 2, 2, 5, 3, true, true);
        Assert.Equal(visibleIds.Skip(2).Take(2), second.Items.Select(a => a.Id));
        var end = await Page(client, "?page=3&pageSize=2" + filter);
        AssertPage(end, 3, 2, 5, 3, true, false);
        Assert.Equal(visibleIds.Skip(4), end.Items.Select(a => a.Id));
        var beyond = await Page(client, "?page=4&pageSize=2" + filter);
        AssertPage(beyond, 4, 2, 5, 3, true, false);
        Assert.Empty(beyond.Items);
        var empty = await Page(client, "?page=1&pageSize=2&category=" + category + "-absent");
        AssertPage(empty, 1, 2, 0, 0, false, false);
        Assert.Empty(empty.Items);
        var exactCategory = await Page(client, "?page=1&pageSize=2&category=" + category + "%20");
        Assert.Empty(exactCategory.Items);

        var unfiltered = await Page(client, "?pageSize=100");
        Assert.True(unfiltered.TotalCount >= 5);
        Assert.Equal(100, unfiltered.PageSize);
        Assert.Equal(visibleIds, unfiltered.Items.Where(a => a.Category == category).Select(a => a.Id));
        Assert.All(unfiltered.Items, a => Assert.True(a.IsPublished));
        var emptyFilter = await Page(client, "?pageSize=100&category=");
        Assert.Equal(visibleIds, emptyFilter.Items.Where(a => a.Category == category).Select(a => a.Id));
        var capped = await Page(client, "?pageSize=101" + filter);
        Assert.Equal(100, capped.PageSize);
        Assert.Equal(5, capped.Items.Count);
        var defaults = await Page(client, "?category=" + category);
        AssertPage(defaults, 1, 10, 5, 1, false, false);

        var categories = (await client.GetFromJsonAsync<List<string>>("/api/v1/blog/categories"))!;
        Assert.Contains(category, categories);
        Assert.DoesNotContain(category + "-draft", categories);
        Assert.DoesNotContain(category + "-deleted", categories);
    }

    [Fact]
    public async Task InvalidBoundsAndOffsets_ReturnValidationErrors()
    {
        await using var factory = new HealthTestFactory();
        using var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        foreach (var query in new[]
        {
            "page=0", "page=-1", "pageSize=0", "pageSize=-1",
            "page=2147483647&pageSize=100", "page=2147483647&pageSize=2147483647",
        })
        {
            using var response = await client.GetAsync("/api/v1/blog/announcements?" + query);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }
        var validLargePage = await Page(client, "?page=2147483647&pageSize=1");
        Assert.Empty(validLargePage.Items);
        Assert.Equal(int.MaxValue, validLargePage.Page);
        Assert.True(validLargePage.HasPrevious);
        Assert.False(validLargePage.HasNext);
    }

    private static async Task<PagedResult<AnnouncementDto>> Page(HttpClient client, string query)
    {
        using var response = await client.GetAsync("/api/v1/blog/announcements" + query);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.CacheControl?.NoStore);
        return (await response.Content.ReadFromJsonAsync<PagedResult<AnnouncementDto>>())!;
    }

    private static void AssertPage(PagedResult<AnnouncementDto> page, int number, int size,
        int count, int totalPages, bool previous, bool next)
    {
        Assert.Equal(number, page.Page);
        Assert.Equal(size, page.PageSize);
        Assert.Equal(count, page.TotalCount);
        Assert.Equal(totalPages, page.TotalPages);
        Assert.Equal(previous, page.HasPrevious);
        Assert.Equal(next, page.HasNext);
    }
}

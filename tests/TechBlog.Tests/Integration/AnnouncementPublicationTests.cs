using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechBlog.Application.Announcements;
using TechBlog.Application.Auth;
using TechBlog.Application.Common.Pagination;
using TechBlog.Domain.Projects;
using TechBlog.Domain.Users;
using TechBlog.Infrastructure.Persistence;
using TechBlog.Infrastructure.Services;
using Xunit;

namespace TechBlog.Tests.Integration;

public sealed class AnnouncementPublicationTests
{
    [Fact]
    public async Task PublicationLifecycle_IsOwnerOnly_AndImmediatelyControlsPublicVisibility()
    {
        await using var factory = new HealthTestFactory();
        using var owner = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        using var other = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        using var publicClient = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        await Login(factory, owner);
        await Login(factory, other);
        var title = "Draft " + Guid.NewGuid().ToString("N");
        var body = new { title, image = "", text = "Test article", category = title, duration = "1", button = "Read" };
        using var created = await owner.PostAsJsonAsync("/api/v1/announcement", body);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var draft = (await created.Content.ReadFromJsonAsync<AnnouncementDto>())!;
        Assert.False(draft.IsPublished);
        Assert.Null(draft.PublishedAt);
        var adminPath = $"/api/v1/announcement/{draft.Id}";
        var publicPaths = new[] { $"/api/v1/blog/announcements/{draft.Id}", $"/api/v1/blog/announcements/by-slug/{draft.Slug}" };
        await AssertPublic(publicClient, publicPaths, HttpStatusCode.NotFound);
        await AssertListings(publicClient, draft, false);
        using (var own = await owner.GetAsync(adminPath)) Assert.Equal(HttpStatusCode.OK, own.StatusCode);
        var owned = await owner.GetFromJsonAsync<List<AnnouncementDto>>("/api/v1/announcement");
        Assert.Contains(owned!, a => a.Id == draft.Id && !a.IsPublished);
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            Assert.False((await db.Announcements.SingleAsync(a => a.Id == draft.Id)).Active);
        }
        using (var denied = await other.PostAsync(adminPath + "/publish", null))
            Assert.Equal(HttpStatusCode.NotFound, denied.StatusCode);
        using (var denied = await other.GetAsync(adminPath))
            Assert.Equal(HttpStatusCode.NotFound, denied.StatusCode);
        using (var missing = await owner.PostAsync("/api/v1/announcement/2147483647/publish", null))
            Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        using (var duplicate = await owner.PostAsJsonAsync("/api/v1/announcement", body))
        {
            Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
            var problem = await duplicate.Content.ReadAsStringAsync();
            Assert.DoesNotContain(title, problem);
            Assert.DoesNotContain("IX_Announcements", problem);
        }

        var published = await Transition(owner, adminPath + "/publish");
        Assert.True(published.IsPublished);
        Assert.NotNull(published.PublishedAt);
        await AssertPublic(publicClient, publicPaths, HttpStatusCode.OK);
        await AssertListings(publicClient, published, true);
        using (var duplicate = await owner.PostAsync(adminPath + "/publish", null))
            Assert.Equal(HttpStatusCode.BadRequest, duplicate.StatusCode);
        using (var denied = await other.PostAsync(adminPath + "/unpublish", null))
            Assert.Equal(HttpStatusCode.NotFound, denied.StatusCode);
        var unpublished = await Transition(owner, adminPath + "/unpublish");
        Assert.False(unpublished.IsPublished);
        Assert.Equal(published.PublishedAt, unpublished.PublishedAt);
        await AssertPublic(publicClient, publicPaths, HttpStatusCode.NotFound);
        await AssertListings(publicClient, unpublished, false);
        using (var duplicate = await owner.PostAsync(adminPath + "/unpublish", null))
            Assert.Equal(HttpStatusCode.BadRequest, duplicate.StatusCode);
        var republished = await Transition(owner, adminPath + "/publish");
        Assert.Equal(published.PublishedAt, republished.PublishedAt);
        using (var deleted = await owner.DeleteAsync(adminPath)) Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);
        using (var missing = await owner.PostAsync(adminPath + "/publish", null))
            Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        using (var missing = await owner.PostAsync(adminPath + "/unpublish", null))
            Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
        await AssertPublic(publicClient, publicPaths, HttpStatusCode.NotFound);
        await AssertListings(publicClient, republished, false);
    }

    private static async Task<AnnouncementDto> Transition(HttpClient client, string path)
    {
        using var response = await client.PostAsync(path, null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<AnnouncementDto>())!;
    }

    private static async Task AssertListings(HttpClient client, AnnouncementDto article, bool visible)
    {
        using var response = await client.GetAsync("/api/v1/blog/announcements?category=" + Uri.EscapeDataString(article.Category));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.CacheControl?.NoStore);
        var page = (await response.Content.ReadFromJsonAsync<PagedResult<AnnouncementDto>>())!;
        Assert.Equal(visible, page.Items.Any(a => a.Id == article.Id));
        using var allResponse = await client.GetAsync("/api/v1/blog/announcements?pageSize=100");
        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
        Assert.True(allResponse.Headers.CacheControl?.NoStore);
        var all = (await allResponse.Content.ReadFromJsonAsync<PagedResult<AnnouncementDto>>())!;
        Assert.All(all.Items, a => Assert.True(a.IsPublished));
        if (!visible) Assert.DoesNotContain(all.Items, a => a.Id == article.Id);
        using var categoriesResponse = await client.GetAsync("/api/v1/blog/categories");
        Assert.True(categoriesResponse.Headers.CacheControl?.NoStore);
        var categories = (await categoriesResponse.Content.ReadFromJsonAsync<List<string>>())!;
        Assert.Equal(visible, categories.Contains(article.Category));
    }

    private static async Task AssertPublic(HttpClient client, string[] paths, HttpStatusCode expected)
    {
        foreach (var path in paths)
        {
            using var response = await client.GetAsync(path);
            Assert.Equal(expected, response.StatusCode);
            Assert.True(response.Headers.CacheControl?.NoStore);
        }
    }

    private static async Task Login(HealthTestFactory factory, HttpClient client)
    {
        var username = "article-" + Guid.NewGuid().ToString("N");
        const string password = "Test-Only-Password-123!";
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            var user = User.Register(Username.Create(username), new BCryptPasswordHasher().Hash(password));
            Assert.True(user.Active);
            db.Users.Add(user);
            await db.SaveChangesAsync();
            var project = Project.Create("Test project", "", "", "Test", "", "", user.Id);
            Assert.True(project.Active);
            db.Projects.Add(project);
            await db.SaveChangesAsync();
        }
        using var login = await client.PostAsJsonAsync("/api/auth/login", new { username, password });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var tokens = (await login.Content.ReadFromJsonAsync<LoginResponse>())!;
        client.DefaultRequestHeaders.Authorization = new("Bearer", tokens.AccessToken);
    }
}

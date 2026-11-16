using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TechBlog.Application.Announcements;
using TechBlog.Application.Announcements.Queries.GetPublishedAnnouncementById;
using TechBlog.Application.Announcements.Queries.GetPublishedAnnouncementBySlug;
using TechBlog.Application.Announcements.Queries.GetPublishedAnnouncements;
using TechBlog.Application.Common.Pagination;
using TechBlog.Domain.Announcements;
using TechBlog.Domain.Users;
using TechBlog.Infrastructure.Persistence;
using Xunit;

namespace TechBlog.Tests.Integration;

public sealed class AnnouncementTranslationTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("pl", true)]
    [InlineData("en", true)]
    [InlineData("EN", false)]
    [InlineData("de", false)]
    [InlineData("", false)]
    public void PublicQueries_ValidateLanguage(string? language, bool valid)
    {
        Assert.Equal(valid, new GetPublishedAnnouncementByIdQueryValidator().Validate(new GetPublishedAnnouncementByIdQuery(1, language)).IsValid);
        Assert.Equal(valid, new GetPublishedAnnouncementBySlugQueryValidator().Validate(new GetPublishedAnnouncementBySlugQuery("slug", language)).IsValid);
        Assert.Equal(valid, new GetPublishedAnnouncementsQueryValidator().Validate(new GetPublishedAnnouncementsQuery(new PaginationParams(), Language: language)).IsValid);
    }

    [Fact]
    public void Mapping_FallsBackAsWholeVariant_AndOwnerUpdateDoesNotAlterEnglish()
    {
        var article = Announcement.Create("Polski", "image", "Polski tekst", "category", "1", "Czytaj", 1);
        var mapper = new AnnouncementMapper();
        Assert.Equal(mapper.ToDto(article), mapper.ToLocalizedDto(article, "en"));
        var translation = new AnnouncementTranslation();
        typeof(AnnouncementTranslation).GetProperty("Title")!.SetValue(translation, "English");
        typeof(AnnouncementTranslation).GetProperty("Text")!.SetValue(translation, "English text");
        typeof(AnnouncementTranslation).GetProperty("Button")!.SetValue(translation, "Read");
        article.Translations.Add(translation);
        var english = mapper.ToLocalizedDto(article, "en");
        Assert.Equal("English", english.Title);
        Assert.Equal("English text", english.Text);
        Assert.Equal("Read", english.Button);
        Assert.Equal(article.Slug, english.Slug);
        Assert.Equal("Polski", mapper.ToDto(article).Title);
        Assert.Equal(mapper.ToDto(article), mapper.ToLocalizedDto(article, "pl"));
        article.Update("Nowy polski", "image", "Nowy tekst", "category", "1", "Dalej", "owner");
        Assert.Equal("English", mapper.ToLocalizedDto(article, "en").Title);
        Assert.Equal("English text", translation.Text);
    }

    [Fact]
    public async Task PublicEndpoints_LocalizeWithoutChangingVisibilityOrCounts()
    {
        await using var factory = new HealthTestFactory();
        using var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        await using var scope = factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        var unique = Guid.NewGuid().ToString("N");
        var user = User.Register(Username.Create("translation-" + unique), "unused-test-hash");
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var article = Announcement.Create("Polski " + unique, "image", "Polski tekst", unique, "1", "Czytaj", user.Id);
        var fallback = Announcement.Create("Fallback " + unique, "image", "Fallback tekst", unique, "1", "Czytaj", user.Id);
        article.Publish("test");
        fallback.Publish("test");
        db.Announcements.AddRange(article, fallback);
        await db.SaveChangesAsync();
        await db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO \"AnnouncementTranslations\" (\"AnnouncementId\", \"Language\", \"Title\", \"Text\", \"Button\") VALUES ({article.Id}, 'en', 'English', 'English text', 'Read')");
        var duplicate = await Assert.ThrowsAsync<PostgresException>(() => db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO \"AnnouncementTranslations\" (\"AnnouncementId\", \"Language\", \"Title\", \"Text\", \"Button\") VALUES ({article.Id}, 'en', 'Duplicate', 'Text', 'Read')"));
        Assert.Equal(PostgresErrorCodes.UniqueViolation, duplicate.SqlState);
        var locale = await Assert.ThrowsAsync<PostgresException>(() => db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO \"AnnouncementTranslations\" (\"AnnouncementId\", \"Language\", \"Title\", \"Text\", \"Button\") VALUES ({article.Id}, 'pl', 'Polski', 'Text', 'Read')"));
        Assert.Equal(PostgresErrorCodes.CheckViolation, locale.SqlState);
        var orphan = await Assert.ThrowsAsync<PostgresException>(() => db.Database.ExecuteSqlRawAsync("INSERT INTO \"AnnouncementTranslations\" (\"AnnouncementId\", \"Language\", \"Title\", \"Text\", \"Button\") VALUES (-1, 'en', 'Orphan', 'Text', 'Read')"));
        Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, orphan.SqlState);
        var required = await Assert.ThrowsAsync<PostgresException>(() => db.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO \"AnnouncementTranslations\" (\"AnnouncementId\", \"Language\", \"Title\", \"Text\", \"Button\") VALUES ({fallback.Id}, 'en', NULL, 'Text', 'Read')"));
        Assert.Equal(PostgresErrorCodes.NotNullViolation, required.SqlState);
        var paths = new[] { $"/api/v1/blog/announcements/{article.Id}", $"/api/v1/blog/announcements/by-slug/{article.Slug}" };
        foreach (var path in paths)
        {
            Assert.Equal(article.Title, (await client.GetFromJsonAsync<AnnouncementDto>(path))!.Title);
            Assert.Equal(article.Title, (await client.GetFromJsonAsync<AnnouncementDto>(path + "?language=pl"))!.Title);
            var english = (await client.GetFromJsonAsync<AnnouncementDto>(path + "?language=en"))!;
            Assert.Equal("English", english.Title);
            Assert.Equal(article.Slug, english.Slug);
            using var invalid = await client.GetAsync(path + "?language=de");
            Assert.Equal(HttpStatusCode.UnprocessableEntity, invalid.StatusCode);
            var problem = await invalid.Content.ReadFromJsonAsync<JsonElement>();
            Assert.True(problem.GetProperty("errors").TryGetProperty("Language", out _));
        }
        var listPath = "/api/v1/blog/announcements?category=" + unique;
        var page = (await client.GetFromJsonAsync<PagedResult<AnnouncementDto>>(listPath + "&language=en"))!;
        Assert.Equal(2, page.TotalCount);
        Assert.Contains(page.Items, a => a.Id == article.Id && a.Title == "English");
        Assert.Contains(page.Items, a => a.Id == fallback.Id && a.Title == fallback.Title && a.Text == fallback.Text && a.Button == fallback.Button);
        using (var invalid = await client.GetAsync(listPath + "&language=de"))
        {
            Assert.Equal(HttpStatusCode.UnprocessableEntity, invalid.StatusCode);
            var problem = await invalid.Content.ReadFromJsonAsync<JsonElement>();
            Assert.True(problem.GetProperty("errors").TryGetProperty("Language", out _));
        }
        using (var unauthorized = await client.GetAsync($"/api/v1/announcement/{article.Id}?language=en")) Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);
        article.Update(article.Title, article.Image, "Updated Polish text", article.Category, article.Duration, "Dalej", "owner");
        await db.SaveChangesAsync();
        var afterUpdate = (await client.GetFromJsonAsync<AnnouncementDto>(paths[0] + "?language=en"))!;
        Assert.Equal("English text", afterUpdate.Text);
        Assert.Equal("Read", afterUpdate.Button);
        Assert.Equal("Updated Polish text", (await client.GetFromJsonAsync<AnnouncementDto>(paths[0]))!.Text);
        article.Unpublish("test");
        await db.SaveChangesAsync();
        foreach (var path in paths)
        foreach (var language in new[] { "pl", "en" })
        {
            using var hidden = await client.GetAsync(path + "?language=" + language);
            Assert.Equal(HttpStatusCode.NotFound, hidden.StatusCode);
        }
        page = (await client.GetFromJsonAsync<PagedResult<AnnouncementDto>>(listPath + "&language=en"))!;
        Assert.Equal(1, page.TotalCount);
        Assert.DoesNotContain(page.Items, a => a.Id == article.Id);
        await db.Database.ExecuteSqlInterpolatedAsync($"DELETE FROM \"Announcements\" WHERE \"Id\" = {article.Id}");
        Assert.False(await db.Set<AnnouncementTranslation>().AnyAsync(t => t.AnnouncementId == article.Id));
    }
}

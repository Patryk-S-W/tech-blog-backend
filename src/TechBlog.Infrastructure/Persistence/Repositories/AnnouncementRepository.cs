using Microsoft.EntityFrameworkCore;
using TechBlog.Domain.Announcements;

namespace TechBlog.Infrastructure.Persistence.Repositories;

public sealed class AnnouncementRepository(DataContext context) : IAnnouncementRepository
{
    public Task<List<Announcement>> GetAllForUserAsync(int userId, CancellationToken ct = default) =>
        context.Announcements
            .Where(a => a.UserId == userId)
            .ToListAsync(ct);

    public Task<Announcement?> GetByIdForUserAsync(int id, int userId, CancellationToken ct = default) =>
        context.Announcements
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId, ct);

    public Task<List<Announcement>> GetAllPublishedAsync(CancellationToken ct = default) =>
        context.Announcements
            .Where(a => a.Active)
            .OrderByDescending(a => a.DateCreated)
            .ToListAsync(ct);

    public Task<List<Announcement>> GetPublishedByCategoryAsync(string category, CancellationToken ct = default) =>
        context.Announcements
            .Where(a => a.Active && a.Category == category)
            .OrderByDescending(a => a.DateCreated)
            .ToListAsync(ct);

    public Task<List<string>> GetDistinctCategoriesAsync(CancellationToken ct = default) =>
        context.Announcements
            .Where(a => a.Active && a.Category != string.Empty)
            .Select(a => a.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(ct);

    public async Task<(List<Announcement> Items, int TotalCount)> GetPublishedPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.Announcements.Where(a => a.Active).OrderByDescending(a => a.DateCreated);
        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, totalCount);
    }

    public Task<Announcement?> GetPublishedByIdAsync(int id, CancellationToken ct = default) =>
        context.Announcements.FirstOrDefaultAsync(a => a.Id == id && a.Active, ct);

    public Task<Announcement?> GetPublishedBySlugAsync(string slug, CancellationToken ct = default) =>
        context.Announcements.FirstOrDefaultAsync(a => a.Slug == slug && a.Active, ct);

    public async Task AddAsync(Announcement announcement, CancellationToken ct = default) =>
        await context.Announcements.AddAsync(announcement, ct);

    public void Remove(Announcement announcement) =>
        context.Announcements.Remove(announcement);
}

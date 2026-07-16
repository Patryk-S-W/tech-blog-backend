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

    public async Task AddAsync(Announcement announcement, CancellationToken ct = default) =>
        await context.Announcements.AddAsync(announcement, ct);

    public void Remove(Announcement announcement) =>
        context.Announcements.Remove(announcement);
}

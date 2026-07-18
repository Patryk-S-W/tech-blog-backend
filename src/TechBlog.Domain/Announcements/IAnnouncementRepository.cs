namespace TechBlog.Domain.Announcements;

public interface IAnnouncementRepository
{
    Task<List<Announcement>> GetAllForUserAsync(int userId, CancellationToken ct = default);

    Task<Announcement?> GetByIdForUserAsync(int id, int userId, CancellationToken ct = default);

    Task<List<Announcement>> GetAllPublishedAsync(CancellationToken ct = default);

    Task<(List<Announcement> Items, int TotalCount)> GetPublishedPagedAsync(int page, int pageSize, CancellationToken ct = default);

    Task<Announcement?> GetPublishedByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(Announcement announcement, CancellationToken ct = default);

    void Remove(Announcement announcement);
}

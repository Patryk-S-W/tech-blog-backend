namespace TechBlog.Domain.Announcements;

public interface IAnnouncementRepository
{
    /// <summary>Every announcement owned by the given user - the
    /// authenticated "manage my posts" view.</summary>
    Task<List<Announcement>> GetAllForUserAsync(int userId, CancellationToken ct = default);

    Task<Announcement?> GetByIdForUserAsync(int id, int userId, CancellationToken ct = default);

    /// <summary>Every published announcement, any author - the public blog
    /// listing anonymous visitors see. Single-author blog today, but this
    /// doesn't assume that.</summary>
    Task<List<Announcement>> GetAllPublishedAsync(CancellationToken ct = default);

    Task<Announcement?> GetPublishedByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(Announcement announcement, CancellationToken ct = default);

    void Remove(Announcement announcement);
}

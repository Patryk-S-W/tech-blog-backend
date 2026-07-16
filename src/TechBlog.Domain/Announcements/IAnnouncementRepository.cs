namespace TechBlog.Domain.Announcements;

public interface IAnnouncementRepository
{
    /// <summary>Every announcement owned by the given user - matches the
    /// current behavior where a user only ever sees their own
    /// announcements, never a global list.</summary>
    Task<List<Announcement>> GetAllForUserAsync(int userId, CancellationToken ct = default);

    Task<Announcement?> GetByIdForUserAsync(int id, int userId, CancellationToken ct = default);

    Task AddAsync(Announcement announcement, CancellationToken ct = default);

    void Remove(Announcement announcement);
}

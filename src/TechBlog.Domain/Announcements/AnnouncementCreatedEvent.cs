using TechBlog.Domain.Common;

namespace TechBlog.Domain.Announcements;

public sealed record AnnouncementCreatedEvent(int AnnouncementId, int UserId) : IDomainEvent;

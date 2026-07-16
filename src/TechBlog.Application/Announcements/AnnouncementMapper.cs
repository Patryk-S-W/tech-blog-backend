using Riok.Mapperly.Abstractions;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements;

[Mapper]
public partial class AnnouncementMapper
{
    // AnnouncementDto intentionally exposes only the public-facing fields -
    // ownership (UserId/User), audit/soft-delete metadata from
    // Common.Entity, and domain events all stay server-side.
    [MapperIgnoreSource(nameof(Announcement.UserId))]
    [MapperIgnoreSource(nameof(Announcement.User))]
    [MapperIgnoreSource(nameof(Announcement.Active))]
    [MapperIgnoreSource(nameof(Announcement.DateCreated))]
    [MapperIgnoreSource(nameof(Announcement.CreatedBy))]
    [MapperIgnoreSource(nameof(Announcement.LastModifiedBy))]
    [MapperIgnoreSource(nameof(Announcement.DateModified))]
    [MapperIgnoreSource(nameof(Announcement.IsDeleted))]
    [MapperIgnoreSource(nameof(Announcement.DeletedBy))]
    [MapperIgnoreSource(nameof(Announcement.DateDeleted))]
    [MapperIgnoreSource(nameof(Announcement.RowVersion))]
    [MapperIgnoreSource(nameof(Announcement.DomainEvents))]
    public partial AnnouncementDto ToDto(Announcement announcement);
}

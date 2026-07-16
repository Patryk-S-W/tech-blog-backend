using Riok.Mapperly.Abstractions;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements;

[Mapper]
public partial class AnnouncementMapper
{
    // AnnouncementDto exposes DateCreated as PublishedAt (public-facing
    // content, unlike the rest of the audit trail below). Everything else
    // - ownership (UserId/User), soft-delete metadata, domain events -
    // stays server-side.
    [MapProperty(nameof(Announcement.DateCreated), nameof(AnnouncementDto.PublishedAt))]
    [MapperIgnoreSource(nameof(Announcement.UserId))]
    [MapperIgnoreSource(nameof(Announcement.User))]
    [MapperIgnoreSource(nameof(Announcement.Active))]
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

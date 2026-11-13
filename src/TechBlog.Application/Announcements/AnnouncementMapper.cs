using Riok.Mapperly.Abstractions;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements;

[Mapper]
public partial class AnnouncementMapper
{
    [MapProperty(nameof(Announcement.Active), nameof(AnnouncementDto.IsPublished))]
    [MapperIgnoreSource(nameof(Announcement.DateCreated))]
    [MapperIgnoreSource(nameof(Announcement.UserId))]
    [MapperIgnoreSource(nameof(Announcement.User))]
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

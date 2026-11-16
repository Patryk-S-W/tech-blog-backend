using Riok.Mapperly.Abstractions;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements;

[Mapper]
public partial class AnnouncementMapper
{
    public AnnouncementDto ToLocalizedDto(Announcement announcement, string? language)
    {
        var dto = ToDto(announcement);
        var translation = language == "en"
            ? announcement.Translations.SingleOrDefault(t => t.Language == "en")
            : null;
        return translation is null ? dto : dto with
        {
            Title = translation.Title,
            Text = translation.Text,
            Button = translation.Button,
        };
    }

    [MapperIgnoreSource(nameof(Announcement.Translations))]
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

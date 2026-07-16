using Riok.Mapperly.Abstractions;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements;

[Mapper]
public partial class AnnouncementMapper
{
    public partial AnnouncementDto ToDto(Announcement announcement);
}

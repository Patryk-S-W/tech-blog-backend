using Riok.Mapperly.Abstractions;

namespace tech_blog_backend.Mappers
{
    [Mapper]
    public partial class AnnouncementMapper
    {
        public partial GetAnnouncementDto ToDto(Announcement announcement);
        public partial Announcement ToEntity(AddAnnouncementDto dto);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tech_blog_backend
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Announcement, GetAnnouncementDto>();
            CreateMap<AddAnnouncementDto, Announcement>();
        }
    }
}
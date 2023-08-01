using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tech_blog_backend.Services.AnnouncementService
{
    public interface IAnnouncementService
    {
        Task<ServiceResponse<List<GetAnnouncementDto>>> GetAllAnnouncements();
        Task<ServiceResponse<GetAnnouncementDto>> GetAnnouncementById(int id);

    }
}
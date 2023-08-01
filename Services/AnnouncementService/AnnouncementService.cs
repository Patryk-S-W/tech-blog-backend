using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tech_blog_backend.Services.AnnouncementService
{
    public class AnnouncementService : IAnnouncementService
    {
        private static List<Announcement> announcements = new List<Announcement> {
            new Announcement()
        };
        private readonly IMapper _mapper;

        public AnnouncementService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetAnnouncementDto>>> GetAllAnnouncements()
        {
            var serviceResponse = new ServiceResponse<List<GetAnnouncementDto>>();
            serviceResponse.Data = announcements.Select(c => _mapper.Map<GetAnnouncementDto>(c)).ToList(); ;
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetAnnouncementDto>> GetAnnouncementById(int id)
        {
            var serviceResponse = new ServiceResponse<GetAnnouncementDto>();
            var announcement = announcements.FirstOrDefault(announcement => announcement.Id == id);

            serviceResponse.Data = _mapper.Map<GetAnnouncementDto>(announcement);
            return serviceResponse;
        }
    }
}
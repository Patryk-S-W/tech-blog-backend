using System.Security.Claims;

namespace tech_blog_backend.Services.AnnouncementService
{
    public class AnnouncementService : IAnnouncementService
    {


        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AnnouncementService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
    .FindFirstValue(ClaimTypes.NameIdentifier)!);

        public async Task<ServiceResponse<List<GetAnnouncementDto>>> GetAllAnnouncements()
        {
            var serviceResponse = new ServiceResponse<List<GetAnnouncementDto>>();
            var dbAnnouncements = await _context.Announcements
                .Where(c => c.User!.Id == GetUserId())
                .ToListAsync();
            serviceResponse.Data = dbAnnouncements.Select(c => _mapper.Map<GetAnnouncementDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetAnnouncementDto>> GetAnnouncementById(int id)
        {

            var serviceResponse = new ServiceResponse<GetAnnouncementDto>();
            var dbAnnouncement = await _context.Announcements
                .Where(c => c.User!.Id == GetUserId())
                .FirstOrDefaultAsync(c => c.Id == id);
            serviceResponse.Data = _mapper.Map<GetAnnouncementDto>(dbAnnouncement);
            return serviceResponse;
        }
    }
}
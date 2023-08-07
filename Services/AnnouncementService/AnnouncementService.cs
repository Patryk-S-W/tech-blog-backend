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

        public async Task<ServiceResponse<List<GetAnnouncementDto>>> AddAnnouncement(AddAnnouncementDto newAnnouncement)
        {
            var serviceResponse = new ServiceResponse<List<GetAnnouncementDto>>();
            var announcement = _mapper.Map<Announcement>(newAnnouncement);
            announcement.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            serviceResponse.Data =
                await _context.Announcements
                    .Where(c => c.User!.Id == GetUserId())
                    .Select(c => _mapper.Map<GetAnnouncementDto>(c))
                    .ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetAnnouncementDto>> UpdateAnnouncement(UpdateAnnouncementDto updatedAnnouncement)
        {
            var serviceResponse = new ServiceResponse<GetAnnouncementDto>();

            try
            {
                var announcement =
                    await _context.Announcements
                        .Include(c => c.User)
                        .FirstOrDefaultAsync(c => c.Id == updatedAnnouncement.Id);
                if (announcement is null || announcement.User!.Id != GetUserId())
                    throw new Exception($"Announcement with Id '{updatedAnnouncement.Id}' not found.");

                announcement.Title = updatedAnnouncement.Title;
                announcement.Image = updatedAnnouncement.Image;
                announcement.Text = updatedAnnouncement.Text;
                announcement.Category = updatedAnnouncement.Category;
                announcement.Duration = updatedAnnouncement.Duration;
                announcement.Button = updatedAnnouncement.Button;

                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetAnnouncementDto>(announcement);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
        public async Task<ServiceResponse<List<GetAnnouncementDto>>> DeleteAnnouncement(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetAnnouncementDto>>();

            try
            {
                var announcement = await _context.Announcements
                    .FirstOrDefaultAsync(c => c.Id == id && c.User!.Id == GetUserId());
                if (announcement is null)
                    throw new Exception($"Announcement with Id '{id}' not found.");

                _context.Announcements.Remove(announcement);

                await _context.SaveChangesAsync();

                serviceResponse.Data =
                    await _context.Announcements
                        .Where(c => c.User!.Id == GetUserId())
                        .Select(c => _mapper.Map<GetAnnouncementDto>(c)).ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}
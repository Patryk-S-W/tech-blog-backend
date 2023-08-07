using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace tech_blog_backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetAnnouncementDto>>>> Get()
        {
            return Ok(await _announcementService.GetAllAnnouncements());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetAnnouncementDto>>> GetById(int id)
        {
            return Ok(await _announcementService.GetAnnouncementById(id));
        }

    }
}
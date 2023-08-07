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

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetAnnouncementDto>>>> AddAnnouncement(AddAnnouncementDto newAnnouncement)
        {
            return Ok(await _announcementService.AddAnnouncement(newAnnouncement));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<GetAnnouncementDto>>>> UpdateAnnouncement(UpdateAnnouncementDto updatedAnnouncement)
        {
            var response = await _announcementService.UpdateAnnouncement(updatedAnnouncement);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetAnnouncementDto>>> DeleteAnnouncement(int id)
        {
            var response = await _announcementService.DeleteAnnouncement(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

    }
}
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechBlog.Application.Announcements;
using TechBlog.Application.Announcements.Commands.CreateAnnouncement;
using TechBlog.Application.Announcements.Commands.DeleteAnnouncement;
using TechBlog.Application.Announcements.Commands.UpdateAnnouncement;
using TechBlog.Application.Announcements.Queries.GetAnnouncementById;
using TechBlog.Application.Announcements.Queries.GetAnnouncements;

namespace TechBlog.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AnnouncementController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AnnouncementDto>>> GetAll(CancellationToken ct) =>
        Ok(await mediator.Send(new GetAnnouncementsQuery(), ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AnnouncementDto>> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetAnnouncementByIdQuery(id), ct));

    [HttpPost]
    public async Task<ActionResult<AnnouncementDto>> Create(CreateAnnouncementCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut]
    public async Task<ActionResult<AnnouncementDto>> Update(UpdateAnnouncementCommand command, CancellationToken ct) =>
        Ok(await mediator.Send(command, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteAnnouncementCommand(id), ct);
        return NoContent();
    }
}

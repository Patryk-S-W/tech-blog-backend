using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechBlog.Application.Announcements;
using TechBlog.Application.Announcements.Queries.GetPublishedAnnouncementById;
using TechBlog.Application.Announcements.Queries.GetPublishedAnnouncements;

namespace TechBlog.WebApi.Controllers;

/// <summary>Public, anonymous-read endpoints for the actual blog - what
/// visitors see. AnnouncementController stays behind [Authorize] for
/// managing your own posts; this is the other half nobody had built yet
/// (every read was scoped to whoever was logged in, which works for a
/// "my posts" dashboard but means an anonymous visitor couldn't see
/// anything).</summary>
[ApiController]
[Route("api/blog")]
[AllowAnonymous]
public sealed class BlogController(IMediator mediator) : ControllerBase
{
    [HttpGet("announcements")]
    public async Task<ActionResult<List<AnnouncementDto>>> GetPublished(CancellationToken ct) =>
        Ok(await mediator.Send(new GetPublishedAnnouncementsQuery(), ct));

    [HttpGet("announcements/{id:int}")]
    public async Task<ActionResult<AnnouncementDto>> GetPublishedById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetPublishedAnnouncementByIdQuery(id), ct));
}

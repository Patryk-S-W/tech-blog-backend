using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechBlog.Application.Announcements;
using TechBlog.Application.Announcements.Queries.GetPublishedAnnouncementById;
using TechBlog.Application.Announcements.Queries.GetPublishedAnnouncements;
using TechBlog.Application.Common.Pagination;

namespace TechBlog.WebApi.Controllers;

[ApiController]
[Route("api/blog")]
[AllowAnonymous]
public sealed class BlogController(IMediator mediator) : ControllerBase
{
    [HttpGet("announcements")]
    public async Task<ActionResult<PagedResult<AnnouncementDto>>> GetPublished(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default) =>
        Ok(await mediator.Send(new GetPublishedAnnouncementsQuery(new PaginationParams { Page = page, PageSize = pageSize }), ct));

    [HttpGet("announcements/{id:int}")]
    public async Task<ActionResult<AnnouncementDto>> GetPublishedById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetPublishedAnnouncementByIdQuery(id), ct));
}

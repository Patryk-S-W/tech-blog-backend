using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using TechBlog.Application.Announcements;
using TechBlog.Application.Announcements.Queries.GetPublishedAnnouncementById;
using TechBlog.Application.Announcements.Queries.GetPublishedAnnouncementBySlug;
using TechBlog.Application.Announcements.Queries.GetPublishedAnnouncements;
using TechBlog.Application.Common.Pagination;
using TechBlog.Domain.Announcements;

namespace TechBlog.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/blog")]
[ApiVersion("1.0")]
[AllowAnonymous]
public sealed class BlogController(IMediator mediator, IAnnouncementRepository repository) : ControllerBase
{
    [HttpGet("announcements")]
    [ResponseCache(Duration = 30, VaryByQueryKeys = ["page", "pageSize", "category"])]
    public async Task<ActionResult<PagedResult<AnnouncementDto>>> GetPublished(
        [FromQuery] PaginationParams query,
        [FromQuery] string? category,
        CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(category))
        {
            var items = await repository.GetPublishedByCategoryAsync(category, ct);
            var mapper = new AnnouncementMapper();
            return Ok(new PagedResult<AnnouncementDto>
            {
                Items = items.Select(mapper.ToDto).ToList(),
                TotalCount = items.Count,
                Page = 1,
                PageSize = items.Count,
            });
        }

        return Ok(await mediator.Send(new GetPublishedAnnouncementsQuery(query), ct));
    }

    [HttpGet("categories")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<List<string>>> GetCategories(CancellationToken ct) =>
        Ok(await repository.GetDistinctCategoriesAsync(ct));

    [HttpGet("announcements/{id:int}")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<AnnouncementDto>> GetPublishedById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetPublishedAnnouncementByIdQuery(id), ct));

    [HttpGet("announcements/by-slug/{slug}")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<AnnouncementDto>> GetPublishedBySlug(string slug, CancellationToken ct) =>
        Ok(await mediator.Send(new GetPublishedAnnouncementBySlugQuery(slug), ct));
}

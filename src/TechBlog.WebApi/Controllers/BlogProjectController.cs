using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechBlog.Application.Projects;
using TechBlog.Application.Projects.Queries.GetPublishedProjectById;
using TechBlog.Application.Projects.Queries.GetPublishedProjects;

namespace TechBlog.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/blog")]
[ApiVersion("1.0")]
[AllowAnonymous]
public sealed class BlogProjectController(IMediator mediator) : ControllerBase
{
    [HttpGet("projects")]
    [ResponseCache(Duration = 30)]
    public async Task<ActionResult<List<ProjectDto>>> GetAll(CancellationToken ct) =>
        Ok(await mediator.Send(new GetPublishedProjectsQuery(), ct));

    [HttpGet("projects/{id:int}")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<ProjectDto>> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetPublishedProjectByIdQuery(id), ct));
}

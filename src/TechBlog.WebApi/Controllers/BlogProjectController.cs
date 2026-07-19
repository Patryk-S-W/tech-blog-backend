using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechBlog.Application.Projects;
using TechBlog.Application.Projects.Queries.GetProjectById;
using TechBlog.Application.Projects.Queries.GetProjects;

namespace TechBlog.WebApi.Controllers;

[ApiController]
[Route("api/blog")]
[AllowAnonymous]
public sealed class BlogProjectController(IMediator mediator) : ControllerBase
{
    [HttpGet("projects")]
    public async Task<ActionResult<List<ProjectDto>>> GetAll(CancellationToken ct) =>
        Ok(await mediator.Send(new GetProjectsQuery(), ct));

    [HttpGet("projects/{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetProjectByIdQuery(id), ct));
}
using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TechBlog.Application.Projects;
using TechBlog.Application.Projects.Commands.CreateProject;
using TechBlog.Application.Projects.Commands.DeleteProject;
using TechBlog.Application.Projects.Commands.UpdateProject;
using TechBlog.Application.Projects.Queries.GetProjectById;
using TechBlog.Application.Projects.Queries.GetProjects;

namespace TechBlog.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/project")]
[ApiVersion("1.0")]
[Authorize]
[EnableRateLimiting("api")]
public sealed class ProjectController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectDto>>> GetAll(CancellationToken ct) =>
        Ok(await mediator.Send(new GetProjectsQuery(), ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDto>> GetById(int id, CancellationToken ct) =>
        Ok(await mediator.Send(new GetProjectByIdQuery(id), ct));

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut]
    public async Task<ActionResult<ProjectDto>> Update(UpdateProjectCommand command, CancellationToken ct) =>
        Ok(await mediator.Send(command, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await mediator.Send(new DeleteProjectCommand(id), ct);
        return NoContent();
    }
}

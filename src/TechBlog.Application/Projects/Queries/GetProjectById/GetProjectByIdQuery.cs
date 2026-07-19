using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Projects;

namespace TechBlog.Application.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(int Id) : IRequest<ProjectDto>;

public sealed class GetProjectByIdQueryHandler(
    IProjectRepository repository,
    ProjectMapper mapper)
    : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    public async ValueTask<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Project {request.Id} not found.");

        return mapper.ToDto(project);
    }
}
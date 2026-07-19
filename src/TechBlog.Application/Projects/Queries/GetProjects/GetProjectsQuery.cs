using Mediator;
using TechBlog.Domain.Projects;

namespace TechBlog.Application.Projects.Queries.GetProjects;

public sealed record GetProjectsQuery : IRequest<List<ProjectDto>>;

public sealed class GetProjectsQueryHandler(
    IProjectRepository repository,
    ProjectMapper mapper)
    : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
{
    public async ValueTask<List<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await repository.GetAllAsync(cancellationToken);
        return projects.Select(mapper.ToDto).ToList();
    }
}
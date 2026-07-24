using Mediator;
using TechBlog.Domain.Projects;

namespace TechBlog.Application.Projects.Queries.GetPublishedProjects;

public sealed record GetPublishedProjectsQuery : IRequest<List<ProjectDto>>;

public sealed class GetPublishedProjectsQueryHandler(
    IProjectRepository repository,
    ProjectMapper mapper)
    : IRequestHandler<GetPublishedProjectsQuery, List<ProjectDto>>
{
    public async ValueTask<List<ProjectDto>> Handle(GetPublishedProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await repository.GetAllPublishedAsync(cancellationToken);
        return projects.Select(mapper.ToDto).ToList();
    }
}

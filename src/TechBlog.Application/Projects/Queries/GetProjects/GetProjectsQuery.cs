using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Projects;

namespace TechBlog.Application.Projects.Queries.GetProjects;

public sealed record GetProjectsQuery : IRequest<List<ProjectDto>>;

public sealed class GetProjectsQueryHandler(
    IProjectRepository repository,
    ICurrentUserService currentUser,
    ProjectMapper mapper)
    : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
{
    public async ValueTask<List<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await repository.GetAllForUserAsync(currentUser.UserId, cancellationToken);
        return projects.Select(mapper.ToDto).ToList();
    }
}

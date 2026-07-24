using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Projects;

namespace TechBlog.Application.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(int Id) : IRequest<ProjectDto>;

public sealed class GetProjectByIdQueryHandler(
    IProjectRepository repository,
    ICurrentUserService currentUser,
    ProjectMapper mapper)
    : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    public async ValueTask<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdForUserAsync(request.Id, currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException($"Project {request.Id} not found.");

        return mapper.ToDto(project);
    }
}

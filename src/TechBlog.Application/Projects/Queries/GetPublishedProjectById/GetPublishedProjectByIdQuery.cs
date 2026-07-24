using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Projects;

namespace TechBlog.Application.Projects.Queries.GetPublishedProjectById;

public sealed record GetPublishedProjectByIdQuery(int Id) : IRequest<ProjectDto>;

public sealed class GetPublishedProjectByIdQueryHandler(
    IProjectRepository repository,
    ProjectMapper mapper)
    : IRequestHandler<GetPublishedProjectByIdQuery, ProjectDto>
{
    public async ValueTask<ProjectDto> Handle(GetPublishedProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await repository.GetPublishedByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Project {request.Id} not found.");

        return mapper.ToDto(project);
    }
}

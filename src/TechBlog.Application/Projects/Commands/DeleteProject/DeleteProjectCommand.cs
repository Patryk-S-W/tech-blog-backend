using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Common;
using TechBlog.Domain.Projects;

namespace TechBlog.Application.Projects.Commands.DeleteProject;

public sealed record DeleteProjectCommand(int Id) : IRequest<bool>;

public sealed class DeleteProjectCommandHandler(
    IProjectRepository repository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProjectCommand, bool>
{
    public async ValueTask<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Project {request.Id} not found.");

        project.MarkDeleted(null);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
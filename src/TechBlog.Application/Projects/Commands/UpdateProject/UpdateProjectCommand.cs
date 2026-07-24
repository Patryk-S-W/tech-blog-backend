using FluentValidation;
using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Projects;

namespace TechBlog.Application.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(
    int Id,
    string Title,
    string Image,
    string ShortDescription,
    string Text,
    string Url,
    string Author
) : IRequest<ProjectDto>;

public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Text).NotEmpty();
    }
}

public sealed class UpdateProjectCommandHandler(
    IProjectRepository repository,
    Domain.Common.IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    ProjectMapper mapper)
    : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    public async ValueTask<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdForUserAsync(request.Id, currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException($"Project {request.Id} not found.");

        project.Update(
            request.Title,
            request.Image,
            request.ShortDescription,
            request.Text,
            request.Url,
            request.Author,
            currentUser.Username);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.ToDto(project);
    }
}

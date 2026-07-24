using FluentValidation;
using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Common;
using TechBlog.Domain.Projects;

namespace TechBlog.Application.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(
    string Title,
    string Image,
    string ShortDescription,
    string Text,
    string Url,
    string Author
) : IRequest<ProjectDto>;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Text).NotEmpty();
    }
}

public sealed class CreateProjectCommandHandler(
    IProjectRepository repository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    ProjectMapper mapper)
    : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    public async ValueTask<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = Project.Create(
            request.Title,
            request.Image,
            request.ShortDescription,
            request.Text,
            request.Url,
            request.Author,
            currentUser.UserId);

        await repository.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.ToDto(project);
    }
}

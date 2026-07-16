using FluentValidation;
using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;
using TechBlog.Domain.Common;

namespace TechBlog.Application.Announcements.Commands.CreateAnnouncement;

public sealed record CreateAnnouncementCommand(
    string Title,
    string Image,
    string Text,
    string Category,
    string Duration,
    string Button
) : IRequest<AnnouncementDto>;

public sealed class CreateAnnouncementCommandValidator : AbstractValidator<CreateAnnouncementCommand>
{
    public CreateAnnouncementCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Text).NotEmpty();
    }
}

public sealed class CreateAnnouncementCommandHandler(
    IAnnouncementRepository repository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    AnnouncementMapper mapper)
    : IRequestHandler<CreateAnnouncementCommand, AnnouncementDto>
{
    public async ValueTask<AnnouncementDto> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var announcement = Announcement.Create(
            request.Title,
            request.Image,
            request.Text,
            request.Category,
            request.Duration,
            request.Button,
            currentUser.UserId);

        await repository.AddAsync(announcement, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.ToDto(announcement);
    }
}

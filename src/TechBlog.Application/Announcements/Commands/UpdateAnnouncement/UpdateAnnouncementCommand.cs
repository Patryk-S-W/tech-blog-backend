using FluentValidation;
using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements.Commands.UpdateAnnouncement;

public sealed record UpdateAnnouncementCommand(
    int Id,
    string Title,
    string Image,
    string Text,
    string Category,
    string Duration,
    string Button
) : IRequest<AnnouncementDto>;

public sealed class UpdateAnnouncementCommandValidator : AbstractValidator<UpdateAnnouncementCommand>
{
    public UpdateAnnouncementCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Text).NotEmpty();
    }
}

public sealed class UpdateAnnouncementCommandHandler(
    IAnnouncementRepository repository,
    Domain.Common.IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    AnnouncementMapper mapper)
    : IRequestHandler<UpdateAnnouncementCommand, AnnouncementDto>
{
    public async ValueTask<AnnouncementDto> Handle(UpdateAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var announcement = await repository.GetByIdForUserAsync(request.Id, currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException($"Announcement {request.Id} not found.");

        announcement.Update(
            request.Title,
            request.Image,
            request.Text,
            request.Category,
            request.Duration,
            request.Button,
            currentUser.Username);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.ToDto(announcement);
    }
}

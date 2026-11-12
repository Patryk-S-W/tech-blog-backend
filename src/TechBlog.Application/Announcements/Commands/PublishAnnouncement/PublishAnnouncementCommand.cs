using FluentValidation;
using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;
using TechBlog.Domain.Common;

namespace TechBlog.Application.Announcements.Commands.PublishAnnouncement;

public sealed record PublishAnnouncementCommand(int Id) : IRequest<AnnouncementDto>;

public sealed class PublishAnnouncementCommandValidator : AbstractValidator<PublishAnnouncementCommand>
{
    public PublishAnnouncementCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public sealed class PublishAnnouncementCommandHandler(
    IAnnouncementRepository repository, IUnitOfWork unitOfWork,
    ICurrentUserService currentUser, AnnouncementMapper mapper)
    : IRequestHandler<PublishAnnouncementCommand, AnnouncementDto>
{
    public async ValueTask<AnnouncementDto> Handle(PublishAnnouncementCommand request, CancellationToken ct)
    {
        var announcement = await repository.GetByIdForUserAsync(request.Id, currentUser.UserId, ct)
            ?? throw new NotFoundException($"Announcement {request.Id} not found.");
        announcement.Publish(currentUser.Username);
        await unitOfWork.SaveChangesAsync(ct);
        return mapper.ToDto(announcement);
    }
}

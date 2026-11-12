using FluentValidation;
using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;
using TechBlog.Domain.Common;

namespace TechBlog.Application.Announcements.Commands.UnpublishAnnouncement;

public sealed record UnpublishAnnouncementCommand(int Id) : IRequest<AnnouncementDto>;

public sealed class UnpublishAnnouncementCommandValidator : AbstractValidator<UnpublishAnnouncementCommand>
{
    public UnpublishAnnouncementCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

public sealed class UnpublishAnnouncementCommandHandler(
    IAnnouncementRepository repository, IUnitOfWork unitOfWork,
    ICurrentUserService currentUser, AnnouncementMapper mapper)
    : IRequestHandler<UnpublishAnnouncementCommand, AnnouncementDto>
{
    public async ValueTask<AnnouncementDto> Handle(UnpublishAnnouncementCommand request, CancellationToken ct)
    {
        var announcement = await repository.GetByIdForUserAsync(request.Id, currentUser.UserId, ct)
            ?? throw new NotFoundException($"Announcement {request.Id} not found.");
        announcement.Unpublish(currentUser.Username);
        await unitOfWork.SaveChangesAsync(ct);
        return mapper.ToDto(announcement);
    }
}

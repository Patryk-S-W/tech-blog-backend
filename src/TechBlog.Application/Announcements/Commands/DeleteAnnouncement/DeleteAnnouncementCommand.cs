using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;
using TechBlog.Domain.Common;

namespace TechBlog.Application.Announcements.Commands.DeleteAnnouncement;

public sealed record DeleteAnnouncementCommand(int Id) : IRequest<bool>;

public sealed class DeleteAnnouncementCommandHandler(
    IAnnouncementRepository repository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser)
    : IRequestHandler<DeleteAnnouncementCommand, bool>
{
    public async ValueTask<bool> Handle(DeleteAnnouncementCommand request, CancellationToken cancellationToken)
    {
        var announcement = await repository.GetByIdForUserAsync(request.Id, currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException($"Announcement {request.Id} not found.");

        // Soft delete (IsDeleted/DeletedBy/DateDeleted via the base Entity) -
        // matches what those columns were already there for, even though
        // the original flat-project code did a hard Remove(). Repository
        // still exposes Remove() for the actual hard-delete case if you
        // ever want one; this command uses the soft path since the audit
        // columns exist and a soft-deleted-but-recoverable record seems
        // like the safer default for a delete button. Say if you'd rather
        // this hard-delete instead.
        announcement.MarkDeleted(currentUser.Username);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

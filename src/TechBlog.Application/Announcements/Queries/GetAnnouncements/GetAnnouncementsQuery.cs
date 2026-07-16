using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements.Queries.GetAnnouncements;

public sealed record GetAnnouncementsQuery : IRequest<List<AnnouncementDto>>;

public sealed class GetAnnouncementsQueryHandler(
    IAnnouncementRepository repository,
    ICurrentUserService currentUser,
    AnnouncementMapper mapper)
    : IRequestHandler<GetAnnouncementsQuery, List<AnnouncementDto>>
{
    public async ValueTask<List<AnnouncementDto>> Handle(GetAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var announcements = await repository.GetAllForUserAsync(currentUser.UserId, cancellationToken);
        return announcements.Select(mapper.ToDto).ToList();
    }
}

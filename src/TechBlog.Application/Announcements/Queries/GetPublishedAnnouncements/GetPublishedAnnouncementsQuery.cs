using Mediator;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements.Queries.GetPublishedAnnouncements;

/// <summary>Public blog listing - no auth, every published announcement
/// from any author.</summary>
public sealed record GetPublishedAnnouncementsQuery : IRequest<List<AnnouncementDto>>;

public sealed class GetPublishedAnnouncementsQueryHandler(
    IAnnouncementRepository repository,
    AnnouncementMapper mapper)
    : IRequestHandler<GetPublishedAnnouncementsQuery, List<AnnouncementDto>>
{
    public async ValueTask<List<AnnouncementDto>> Handle(GetPublishedAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var announcements = await repository.GetAllPublishedAsync(cancellationToken);
        return announcements.Select(mapper.ToDto).ToList();
    }
}

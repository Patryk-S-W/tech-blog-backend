using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements.Queries.GetPublishedAnnouncementById;

public sealed record GetPublishedAnnouncementByIdQuery(int Id) : IRequest<AnnouncementDto>;

public sealed class GetPublishedAnnouncementByIdQueryHandler(
    IAnnouncementRepository repository,
    AnnouncementMapper mapper)
    : IRequestHandler<GetPublishedAnnouncementByIdQuery, AnnouncementDto>
{
    public async ValueTask<AnnouncementDto> Handle(GetPublishedAnnouncementByIdQuery request, CancellationToken cancellationToken)
    {
        var announcement = await repository.GetPublishedByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Announcement {request.Id} not found.");

        return mapper.ToDto(announcement);
    }
}

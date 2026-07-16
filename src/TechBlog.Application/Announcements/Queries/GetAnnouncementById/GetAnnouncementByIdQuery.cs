using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements.Queries.GetAnnouncementById;

public sealed record GetAnnouncementByIdQuery(int Id) : IRequest<AnnouncementDto>;

public sealed class GetAnnouncementByIdQueryHandler(
    IAnnouncementRepository repository,
    ICurrentUserService currentUser,
    AnnouncementMapper mapper)
    : IRequestHandler<GetAnnouncementByIdQuery, AnnouncementDto>
{
    public async ValueTask<AnnouncementDto> Handle(GetAnnouncementByIdQuery request, CancellationToken cancellationToken)
    {
        var announcement = await repository.GetByIdForUserAsync(request.Id, currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException($"Announcement {request.Id} not found.");

        return mapper.ToDto(announcement);
    }
}

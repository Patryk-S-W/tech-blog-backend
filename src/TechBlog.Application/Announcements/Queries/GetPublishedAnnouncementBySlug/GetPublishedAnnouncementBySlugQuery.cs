using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements.Queries.GetPublishedAnnouncementBySlug;

public sealed record GetPublishedAnnouncementBySlugQuery(string Slug) : IRequest<AnnouncementDto>;

public sealed class GetPublishedAnnouncementBySlugQueryHandler(
    IAnnouncementRepository repository,
    AnnouncementMapper mapper)
    : IRequestHandler<GetPublishedAnnouncementBySlugQuery, AnnouncementDto>
{
    public async ValueTask<AnnouncementDto> Handle(GetPublishedAnnouncementBySlugQuery request, CancellationToken cancellationToken)
    {
        var announcement = await repository.GetPublishedBySlugAsync(request.Slug, cancellationToken)
            ?? throw new NotFoundException($"Announcement '{request.Slug}' not found.");

        return mapper.ToDto(announcement);
    }
}

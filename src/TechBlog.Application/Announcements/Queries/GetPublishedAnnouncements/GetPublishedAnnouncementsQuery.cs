using Mediator;
using TechBlog.Application.Common.Pagination;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements.Queries.GetPublishedAnnouncements;

public sealed record GetPublishedAnnouncementsQuery(PaginationParams Params) : IRequest<PagedResult<AnnouncementDto>>;

public sealed class GetPublishedAnnouncementsQueryHandler(
    IAnnouncementRepository repository,
    AnnouncementMapper mapper)
    : IRequestHandler<GetPublishedAnnouncementsQuery, PagedResult<AnnouncementDto>>
{
    public async ValueTask<PagedResult<AnnouncementDto>> Handle(GetPublishedAnnouncementsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await repository.GetPublishedPagedAsync(
            request.Params.Page,
            request.Params.PageSize,
            cancellationToken);

        return new PagedResult<AnnouncementDto>
        {
            Items = items.Select(mapper.ToDto).ToList(),
            TotalCount = totalCount,
            Page = request.Params.Page,
            PageSize = request.Params.PageSize,
        };
    }
}

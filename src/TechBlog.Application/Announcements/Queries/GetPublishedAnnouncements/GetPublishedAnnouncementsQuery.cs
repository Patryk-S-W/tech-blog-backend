using Mediator;
using FluentValidation;
using TechBlog.Application.Common.Pagination;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements.Queries.GetPublishedAnnouncements;

public sealed record GetPublishedAnnouncementsQuery(PaginationParams Params, string? Category = null) : IRequest<PagedResult<AnnouncementDto>>;

public sealed class GetPublishedAnnouncementsQueryValidator : AbstractValidator<GetPublishedAnnouncementsQuery>
{
    public GetPublishedAnnouncementsQueryValidator()
    {
        RuleFor(x => x.Params.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Params.PageSize).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Params).Must(p => ((long)p.Page - 1) * p.PageSize <= int.MaxValue)
            .WithMessage("The requested page offset is too large.");
    }
}

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
            request.Category,
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

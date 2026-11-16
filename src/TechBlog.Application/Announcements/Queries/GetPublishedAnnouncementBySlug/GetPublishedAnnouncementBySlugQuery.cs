using Mediator;
using FluentValidation;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements.Queries.GetPublishedAnnouncementBySlug;

public sealed record GetPublishedAnnouncementBySlugQuery(string Slug, string? Language = null) : IRequest<AnnouncementDto>;

public sealed class GetPublishedAnnouncementBySlugQueryValidator : AbstractValidator<GetPublishedAnnouncementBySlugQuery>
{
    public GetPublishedAnnouncementBySlugQueryValidator()
    {
        RuleFor(x => x.Language).Must(language => language is null or "pl" or "en")
            .WithMessage("Language must be pl or en.");
    }
}

public sealed class GetPublishedAnnouncementBySlugQueryHandler(
    IAnnouncementRepository repository,
    AnnouncementMapper mapper)
    : IRequestHandler<GetPublishedAnnouncementBySlugQuery, AnnouncementDto>
{
    public async ValueTask<AnnouncementDto> Handle(GetPublishedAnnouncementBySlugQuery request, CancellationToken cancellationToken)
    {
        var announcement = await repository.GetPublishedBySlugAsync(request.Slug, cancellationToken)
            ?? throw new NotFoundException($"Announcement '{request.Slug}' not found.");

        return mapper.ToLocalizedDto(announcement, request.Language);
    }
}

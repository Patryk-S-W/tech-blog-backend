using Mediator;
using FluentValidation;
using TechBlog.Application.Common;
using TechBlog.Domain.Announcements;

namespace TechBlog.Application.Announcements.Queries.GetPublishedAnnouncementById;

public sealed record GetPublishedAnnouncementByIdQuery(int Id, string? Language = null) : IRequest<AnnouncementDto>;

public sealed class GetPublishedAnnouncementByIdQueryValidator : AbstractValidator<GetPublishedAnnouncementByIdQuery>
{
    public GetPublishedAnnouncementByIdQueryValidator()
    {
        RuleFor(x => x.Language).Must(language => language is null or "pl" or "en")
            .WithMessage("Language must be pl or en.");
    }
}

public sealed class GetPublishedAnnouncementByIdQueryHandler(
    IAnnouncementRepository repository,
    AnnouncementMapper mapper)
    : IRequestHandler<GetPublishedAnnouncementByIdQuery, AnnouncementDto>
{
    public async ValueTask<AnnouncementDto> Handle(GetPublishedAnnouncementByIdQuery request, CancellationToken cancellationToken)
    {
        var announcement = await repository.GetPublishedByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Announcement {request.Id} not found.");

        return mapper.ToLocalizedDto(announcement, request.Language);
    }
}

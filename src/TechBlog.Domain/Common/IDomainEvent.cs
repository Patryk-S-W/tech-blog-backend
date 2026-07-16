namespace TechBlog.Domain.Common;

/// <summary>Marker interface for domain events. Concrete events are plain
/// records implementing this - e.g.
/// <c>public sealed record AnnouncementCreatedEvent(int AnnouncementId) : IDomainEvent;</c>
/// </summary>
public interface IDomainEvent;

public interface IHasDomainEvents
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}

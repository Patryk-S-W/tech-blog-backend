namespace TechBlog.Domain.Common;

/// <summary>
///     Base class for entities that have their own identity and lifecycle.
///     Setters are private/protected on purpose - state changes go through
///     named methods on the concrete entity (e.g. <c>Announcement.Publish()</c>),
///     not free-form property assignment from outside the aggregate.
/// </summary>
public abstract class Entity : IEntity, IHasDomainEvents
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity()
    {
        Active = true;
        DateCreated = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public bool Active { get; private set; }
    public DateTime? DateCreated { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public DateTime DateModified { get; private set; } = DateTime.UtcNow;
    public bool IsDeleted { get; private set; }
    public string? DeletedBy { get; private set; }
    public DateTime? DateDeleted { get; private set; }
    public byte[]? RowVersion { get; private set; }

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>For concrete entities that expose publish/unpublish-style
    /// behavior built on top of Active - keeps the setter out of the public
    /// surface while still letting subclasses flip it through a named
    /// method of their own (see Announcement.Publish/Unpublish).</summary>
    protected void SetActive(bool active) => Active = active;

    /// <summary>Soft-deletes the entity. There's no undelete on purpose - that's
    /// a product decision (restore flow, audit trail for it, etc.) nobody has
    /// asked for yet.</summary>
    public void MarkDeleted(string? deletedBy)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DateDeleted = DateTime.UtcNow;
        Touch(deletedBy);
    }

    /// <summary>Call from concrete entities whenever a mutating method changes
    /// state, so DateModified/LastModifiedBy stay accurate.</summary>
    protected void Touch(string? modifiedBy)
    {
        DateModified = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}

namespace tech_blog_backend.Common.Contracts;

public interface IEntity
{
    /// <summary>
    ///     This property is used to get and set the Id of the object
    /// </summary>
    /// <remarks>Do not manually set this property. It is handled automatically.</remarks>
    public int Id { get; set; }

    public bool Active { get; set; }

    /// <summary>
    ///     This property is used to get and set the Date & Time the entity was created
    /// </summary>
    public DateTime? DateCreated { get; set; }

    /// <summary>
    ///     This property is used to get and set the ID of the user who created the entity
    /// </summary>
    /// <remarks>Nullable: nothing in this codebase populates it yet - there's no
    /// SaveChanges interceptor or controller-level wiring that sets it from the
    /// authenticated caller. Pretending it's always set (e.g. via `required`)
    /// would just move today's warning to every entity-construction call site
    /// without actually closing the gap.</remarks>
    public string? CreatedBy { get; set; }

    /// <summary>
    ///     This property is used to get and set the ID of the user who last modified the entity
    /// </summary>
    /// <remarks>Nullable - see CreatedBy.</remarks>
    public string? LastModifiedBy { get; set; }

    /// <summary>
    ///     This property is used to get and set the Date & Time the entity was last changed
    /// </summary>
    public DateTime DateModified { get; set; }

    /// <summary>
    ///     This property is used to get and set whether the entity has been deleted
    /// </summary>
    /// <remarks>If <c>true</c> a query filter is applied to ignore the entity</remarks>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     This property is used to get and set the ID of the user who deleted the entity
    /// </summary>
    /// <remarks>Nullable: only meaningful once IsDeleted is true.</remarks>
    public string? DeletedBy { get; set; }

    /// <summary>
    ///     This property is used to get and set the Date & Time the entity was deleted on
    /// </summary>
    public DateTime? DateDeleted { get; set; }

    /// <summary>
    ///     This property is to get and set the Concurrency Token for EntityFramework Core
    /// </summary>
    /// <remarks>Nullable: EF Core only assigns this on first save via the database;
    /// before that (e.g. right after `new Announcement()`), it's genuinely null.</remarks>
    /// <seealso href="https://docs.microsoft.com/en-us/ef/core/saving/concurrency">Concurrency</seealso>
    public byte[]? RowVersion { get; set; }
}

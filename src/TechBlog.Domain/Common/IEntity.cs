namespace TechBlog.Domain.Common;

public interface IEntity
{
    /// <summary>
    ///     This property is used to get and set the Id of the object
    /// </summary>
    /// <remarks>Do not manually set this property. It is handled automatically.</remarks>
    public int Id { get; }

    public bool Active { get; }

    /// <summary>
    ///     This property is used to get and set the Date &amp; Time the entity was created
    /// </summary>
    public DateTime? DateCreated { get; }

    /// <summary>
    ///     This property is used to get and set the ID of the user who created the entity
    /// </summary>
    /// <remarks>Nullable: nothing populates this yet - see the same note on
    /// LastModifiedBy/DeletedBy. A real audit trail needs a SaveChanges
    /// interceptor reading the current user, which doesn't exist in this
    /// codebase yet.</remarks>
    public string? CreatedBy { get; }

    public string? LastModifiedBy { get; }

    public DateTime DateModified { get; }

    /// <summary>
    ///     This property is used to get and set whether the entity has been deleted
    /// </summary>
    /// <remarks>If <c>true</c> a query filter is applied to ignore the entity</remarks>
    public bool IsDeleted { get; }

    public string? DeletedBy { get; }

    public DateTime? DateDeleted { get; }

    /// <summary>
    ///     Concurrency token for EF Core. Null until the database assigns it on
    ///     first save.
    /// </summary>
    /// <seealso href="https://docs.microsoft.com/en-us/ef/core/saving/concurrency">Concurrency</seealso>
    public byte[]? RowVersion { get; }
}

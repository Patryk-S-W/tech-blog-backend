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
    public string CreatedBy { get; set; }

    /// <summary>
    ///     This property is used to get and set the ID of the user who last modified the entity
    /// </summary>
    public string LastModifiedBy { get; set; }

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
    public string DeletedBy { get; set; }

    /// <summary>
    ///     This property is used to get and set the Date & Time the entity was deleted on
    /// </summary>
    public DateTime? DateDeleted { get; set; }

    /// <summary>
    ///     This property is to get and set the Concurrency Token for EntityFramework Core
    /// </summary>
    /// <seealso href="https://docs.microsoft.com/en-us/ef/core/saving/concurrency">Concurrency</seealso>
    public byte[] RowVersion { get; set; }
}
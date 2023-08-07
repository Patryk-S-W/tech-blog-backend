using System.ComponentModel.DataAnnotations;
using tech_blog_backend.Common.Contracts;

namespace tech_blog_backend.Common;

public class Entity : IEntity
{
    public int Id { get; set; }
    public bool Active { get; set; }
    public DateTime? DateCreated { get; set; }
    public string CreatedBy { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime DateModified { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
    public string DeletedBy { get; set; }
    public DateTime? DateDeleted { get; set; }

    [Timestamp] public byte[] RowVersion { get; set; }
}
using Riok.Mapperly.Abstractions;
using TechBlog.Domain.Projects;

namespace TechBlog.Application.Projects;

[Mapper]
public partial class ProjectMapper
{
    [MapperIgnoreSource(nameof(Project.Active))]
    [MapperIgnoreSource(nameof(Project.UserId))]
    [MapperIgnoreSource(nameof(Project.User))]
    [MapperIgnoreSource(nameof(Project.DateCreated))]
    [MapperIgnoreSource(nameof(Project.CreatedBy))]
    [MapperIgnoreSource(nameof(Project.LastModifiedBy))]
    [MapperIgnoreSource(nameof(Project.DateModified))]
    [MapperIgnoreSource(nameof(Project.IsDeleted))]
    [MapperIgnoreSource(nameof(Project.DeletedBy))]
    [MapperIgnoreSource(nameof(Project.DateDeleted))]
    [MapperIgnoreSource(nameof(Project.RowVersion))]
    [MapperIgnoreSource(nameof(Project.DomainEvents))]
    public partial ProjectDto ToDto(Project project);
}

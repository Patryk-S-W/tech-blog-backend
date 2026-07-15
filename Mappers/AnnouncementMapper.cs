using Riok.Mapperly.Abstractions;

namespace tech_blog_backend.Mappers
{
    [Mapper]
    public partial class AnnouncementMapper
    {
        // GetAnnouncementDto intentionally exposes only the public-facing
        // fields - the audit/soft-delete metadata from Common.Entity and
        // the User navigation property stay server-side.
        [MapperIgnoreSource(nameof(Announcement.User))]
        [MapperIgnoreSource(nameof(Announcement.Active))]
        [MapperIgnoreSource(nameof(Announcement.DateCreated))]
        [MapperIgnoreSource(nameof(Announcement.CreatedBy))]
        [MapperIgnoreSource(nameof(Announcement.LastModifiedBy))]
        [MapperIgnoreSource(nameof(Announcement.DateModified))]
        [MapperIgnoreSource(nameof(Announcement.IsDeleted))]
        [MapperIgnoreSource(nameof(Announcement.DeletedBy))]
        [MapperIgnoreSource(nameof(Announcement.DateDeleted))]
        [MapperIgnoreSource(nameof(Announcement.RowVersion))]
        public partial GetAnnouncementDto ToDto(Announcement announcement);

        // AddAnnouncementDto is the inbound request shape - it never
        // carries the entity's Id, User, or audit/soft-delete fields;
        // those are set by the service (Id by EF, User from the
        // authenticated caller, audit fields default/DB-generated).
        [MapperIgnoreTarget(nameof(Announcement.Id))]
        [MapperIgnoreTarget(nameof(Announcement.User))]
        [MapperIgnoreTarget(nameof(Announcement.Active))]
        [MapperIgnoreTarget(nameof(Announcement.DateCreated))]
        [MapperIgnoreTarget(nameof(Announcement.CreatedBy))]
        [MapperIgnoreTarget(nameof(Announcement.LastModifiedBy))]
        [MapperIgnoreTarget(nameof(Announcement.DateModified))]
        [MapperIgnoreTarget(nameof(Announcement.IsDeleted))]
        [MapperIgnoreTarget(nameof(Announcement.DeletedBy))]
        [MapperIgnoreTarget(nameof(Announcement.DateDeleted))]
        [MapperIgnoreTarget(nameof(Announcement.RowVersion))]
        public partial Announcement ToEntity(AddAnnouncementDto dto);
    }
}

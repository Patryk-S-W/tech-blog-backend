using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechBlog.Domain.Announcements;

namespace TechBlog.Infrastructure.Persistence.Configurations;

public sealed class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("Announcements");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title).IsRequired();
        builder.Property(a => a.Slug).IsRequired();
        builder.HasIndex(a => a.Slug).IsUnique();
        builder.Property(a => a.Image).IsRequired();
        builder.Property(a => a.Text).IsRequired();
        builder.Property(a => a.Category).IsRequired();
        builder.Property(a => a.Duration).IsRequired();
        builder.Property(a => a.Button).IsRequired();

        // Explicit FK on an explicit UserId property - the original flat
        // project only had a User navigation with no UserId CLR property,
        // so EF Core created 'Announcement.UserId' as a shadow property
        // (confirmed in your migration output: "The property
        // 'Announcement.UserId' was created in shadow state"). It worked,
        // but a shadow FK you can't reference from C# is easy to forget
        // about; UserId is now a first-class property on the entity.
        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft-deleted announcements never come back from a normal query -
        // DeleteAnnouncementCommand sets IsDeleted via Entity.MarkDeleted()
        // instead of actually removing the row.
        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.Ignore(a => a.DomainEvents);
    }
}

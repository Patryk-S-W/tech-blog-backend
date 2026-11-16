using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechBlog.Domain.Announcements;

namespace TechBlog.Infrastructure.Persistence.Configurations;

public sealed class AnnouncementTranslationConfiguration : IEntityTypeConfiguration<AnnouncementTranslation>
{
    public void Configure(EntityTypeBuilder<AnnouncementTranslation> builder)
    {
        builder.ToTable("AnnouncementTranslations", table =>
            table.HasCheckConstraint("CK_AnnouncementTranslations_Language", "\"Language\" = 'en'"));
        builder.HasKey(t => new { t.AnnouncementId, t.Language });
        builder.Property(t => t.Language).IsRequired();
        builder.Property(t => t.Title).IsRequired();
        builder.Property(t => t.Text).IsRequired();
        builder.Property(t => t.Button).IsRequired();
        builder.HasOne<Announcement>().WithMany(a => a.Translations)
            .HasForeignKey(t => t.AnnouncementId).OnDelete(DeleteBehavior.Cascade);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechBlog.Domain.Users;

namespace TechBlog.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        // Username is a Value Object (validated wrapper around a string) -
        // a converter maps it straight to a single text column instead of
        // EF Core trying to treat it as an owned entity type.
        builder.Property(u => u.Username)
            .HasConversion(username => username.Value, value => Username.Create(value))
            .HasColumnName("Username")
            .IsRequired();

        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Role).IsRequired().HasMaxLength(50).HasDefaultValue("User");
        builder.Property(u => u.FailedLoginAttempts).HasDefaultValue(0);
        builder.Property(u => u.LockoutEnd).IsRequired(false);

        builder.Ignore(u => u.DomainEvents);
        builder.Ignore(u => u.IsLockedOut);
    }
}

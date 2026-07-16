using TechBlog.Domain.Common;

namespace TechBlog.Domain.Users;

/// <summary>Aggregate root. Deliberately doesn't hold a collection of
/// Announcements - Announcement is its own aggregate root (it's loaded,
/// queried, and mutated independently by Id, never through User), linked
/// back only by Announcement.UserId. Coupling them here would make this
/// one aggregate's consistency boundary include the other's, which isn't
/// how either is actually used.</summary>
public sealed class User : Entity, IAggregateRoot
{
    // EF Core materializes entities via this - never call it from app code.
    private User() { }

    public Username Username { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;

    /// <param name="passwordHash">Already hashed - hashing is an
    /// infrastructure concern (BCrypt today), the domain just stores and
    /// compares hashes via <see cref="IPasswordHasher"/>.</param>
    public static User Register(Username username, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash cannot be empty.");

        return new User
        {
            Username = username,
            PasswordHash = passwordHash,
        };
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new DomainException("Password hash cannot be empty.");

        PasswordHash = newPasswordHash;
        Touch(Username.Value);
    }
}

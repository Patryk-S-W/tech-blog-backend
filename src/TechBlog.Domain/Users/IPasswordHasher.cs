namespace TechBlog.Domain.Users;

/// <summary>Domain-level abstraction over password hashing - keeps
/// BCrypt.Net (or whatever comes next) out of the Domain project entirely.
/// Implemented in Infrastructure.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

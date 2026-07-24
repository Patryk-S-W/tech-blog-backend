using TechBlog.Domain.Common;

namespace TechBlog.Domain.Users;

public sealed class RefreshToken : Entity, IAggregateRoot
{
    private RefreshToken() { }

    public string Token { get; private set; } = string.Empty;
    public int UserId { get; private set; }
    public User? User { get; private set; }
    public DateTime Expires { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= Expires;

    public static RefreshToken Create(string token, int userId, TimeSpan lifetime)
    {
        return new RefreshToken
        {
            Token = token,
            UserId = userId,
            Expires = DateTime.UtcNow.Add(lifetime),
        };
    }
}

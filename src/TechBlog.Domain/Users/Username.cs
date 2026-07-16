using TechBlog.Domain.Common;

namespace TechBlog.Domain.Users;

/// <summary>Value object - two Usernames with the same Value are equal
/// (record gives us that for free), and a Username can't exist in an
/// invalid state once constructed.</summary>
public sealed record Username
{
    public const int MaxLength = 50;

    public string Value { get; }

    private Username(string value) => Value = value;

    public static Username Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Username cannot be empty.");

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
            throw new DomainException($"Username cannot exceed {MaxLength} characters.");

        return new Username(trimmed);
    }

    public override string ToString() => Value;
}

using TechBlog.Domain.Common;

namespace TechBlog.Domain.Categories;

/// <summary>Carried over from the flat project as-is - not wired to
/// Announcement.Category (still a free-text string there), no repository,
/// nothing queries this. Left dormant rather than force-connecting it
/// during this restructure; that's separate work (making blog categories
/// real instead of placeholder strings) that hasn't been scoped yet.</summary>
public sealed class Category : Entity, IAggregateRoot
{
    private Category() { }

    public string Name { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;

    public static Category Create(string name, string url)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name cannot be empty.");

        return new Category { Name = name.Trim(), Url = url };
    }
}

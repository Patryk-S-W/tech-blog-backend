using TechBlog.Domain.Common;

namespace TechBlog.Domain.Projects;

/// <summary>Renamed from the flat project's "Projects" class - a plural
/// name for a class representing one project isn't idiomatic. Fields
/// carried over unchanged. ProjectController/ProjectService are still
/// stubs upstream (no real CRUD to preserve), so this stays a plain data
/// holder for now rather than growing behavior nobody asked for yet -
/// this is the entity that's meant to eventually become the standalone
/// "wiki" you mentioned, separate from the blog, with the ability to link
/// to it from announcements. Not building that here.</summary>
public sealed class Project : Entity, IAggregateRoot
{
    private Project() { }

    public string Title { get; private set; } = string.Empty;
    public string Image { get; private set; } = string.Empty;
    public string ShortDescription { get; private set; } = string.Empty;
    public string Text { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;

    public static Project Create(
        string title,
        string image,
        string shortDescription,
        string text,
        string url,
        string author)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Project title cannot be empty.");

        return new Project
        {
            Title = title.Trim(),
            Image = image,
            ShortDescription = shortDescription,
            Text = text,
            Url = url,
            Author = author,
        };
    }
}

using TechBlog.Domain.Common;

namespace TechBlog.Domain.Projects;

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

    public void Update(
        string title,
        string image,
        string shortDescription,
        string text,
        string url,
        string author)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Project title cannot be empty.");

        Title = title.Trim();
        Image = image;
        ShortDescription = shortDescription;
        Text = text;
        Url = url;
        Author = author;
    }
}
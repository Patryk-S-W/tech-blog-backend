namespace TechBlog.Application.Projects;

public sealed record ProjectDto(
    int Id,
    string Title,
    string Image,
    string ShortDescription,
    string Text,
    string Url,
    string Author
);
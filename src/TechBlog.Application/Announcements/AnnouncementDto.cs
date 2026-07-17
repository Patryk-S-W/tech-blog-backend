namespace TechBlog.Application.Announcements;

public sealed record AnnouncementDto(
    int Id,
    string Title,
    string Image,
    string Text,
    string Category,
    string Duration,
    string Button,
    DateTime? PublishedAt
);

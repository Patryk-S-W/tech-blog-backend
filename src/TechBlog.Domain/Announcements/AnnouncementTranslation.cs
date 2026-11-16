namespace TechBlog.Domain.Announcements;

public sealed class AnnouncementTranslation
{
    public int AnnouncementId { get; private set; }
    public string Language { get; private set; } = "en";
    public string Title { get; private set; } = string.Empty;
    public string Text { get; private set; } = string.Empty;
    public string Button { get; private set; } = string.Empty;
}

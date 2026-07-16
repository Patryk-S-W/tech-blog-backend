using TechBlog.Domain.Common;
using TechBlog.Domain.Users;

namespace TechBlog.Domain.Announcements;

public sealed class Announcement : Entity, IAggregateRoot
{
    // EF Core materializes entities via this - never call it from app code.
    private Announcement() { }

    public string Title { get; private set; } = string.Empty;
    public string Image { get; private set; } = string.Empty;
    public string Text { get; private set; } = string.Empty;

    /// <summary>Free-text category today (matches current behavior) - not a
    /// real Category FK. See Categories/Category.cs for why.</summary>
    public string Category { get; private set; } = string.Empty;

    public string Duration { get; private set; } = string.Empty;
    public string Button { get; private set; } = string.Empty;

    public int UserId { get; private set; }
    public User? User { get; private set; }

    public static Announcement Create(
        string title,
        string image,
        string text,
        string category,
        string duration,
        string button,
        int userId)
    {
        ValidateTitle(title);

        var announcement = new Announcement
        {
            Title = title.Trim(),
            Image = image,
            Text = text,
            Category = category,
            Duration = duration,
            Button = button,
            UserId = userId,
        };

        // Note for whenever this actually gets dispatched: announcement.Id
        // is still 0 here - the database hasn't assigned one yet, that
        // only happens on SaveChanges. A handler reacting to this would
        // need the Id to come from after persistence, not from this event
        // as raised.
        announcement.AddDomainEvent(new AnnouncementCreatedEvent(announcement.Id, userId));

        return announcement;
    }

    public void Update(
        string title,
        string image,
        string text,
        string category,
        string duration,
        string button,
        string? modifiedBy)
    {
        ValidateTitle(title);

        Title = title.Trim();
        Image = image;
        Text = text;
        Category = category;
        Duration = duration;
        Button = button;

        Touch(modifiedBy);
    }

    /// <summary>Uses the existing Active flag as the published/unpublished
    /// switch - that's what it was already doing implicitly (nothing ever
    /// set it to false, so every announcement was implicitly "active" by
    /// default), just named now.</summary>
    public void Publish(string? modifiedBy)
    {
        if (Active)
            throw new DomainException("Announcement is already published.");

        SetActive(true);
        Touch(modifiedBy);
    }

    public void Unpublish(string? modifiedBy)
    {
        if (!Active)
            throw new DomainException("Announcement is already unpublished.");

        SetActive(false);
        Touch(modifiedBy);
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Announcement title cannot be empty.");
    }
}

using System.ComponentModel.DataAnnotations;

namespace tech_blog_backend.Dtos.Announcement
{
    public class UpdateAnnouncementDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Button { get; set; } = string.Empty;
    }
}
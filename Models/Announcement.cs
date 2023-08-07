using tech_blog_backend.Common;
using tech_blog_backend.Common.Contracts;
namespace tech_blog_backend.Models
{
    public class Announcement : Entity
    {
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Button { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}
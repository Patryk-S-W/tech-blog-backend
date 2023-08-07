using tech_blog_backend.Common;
using tech_blog_backend.Common.Contracts;

namespace tech_blog_backend.Models
{
    public class Projects : Entity
    {
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
    }
}
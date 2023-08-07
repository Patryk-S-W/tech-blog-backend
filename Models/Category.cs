using tech_blog_backend.Common;
using tech_blog_backend.Common.Contracts;

namespace tech_blog_backend.Models
{
    public class Category : Entity
    {
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
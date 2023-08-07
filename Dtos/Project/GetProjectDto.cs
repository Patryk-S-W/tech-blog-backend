namespace tech_blog_backend.Dtos.Project
{
    public class GetProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
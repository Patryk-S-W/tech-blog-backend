using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tech_blog_backend.Dtos.Project
{
    public class GetProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Date { get; set; }
        public string Text { get; set; }
    }
}
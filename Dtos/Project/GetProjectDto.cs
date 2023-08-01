using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace tech_blog_backend.Dtos.Project
{
    public class GetProjectDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
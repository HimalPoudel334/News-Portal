using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsportal.Models
{
    public class News
    {
        public int Id { get; set; }
        public int Count { get; set; } = 0;
        public DateTime PublishedDate { get; set; }
        public Category Category { get; set; }
        public Reporter Reporter { get; set; }
        public Reporter LastEditedBy { get; set; } = null;

        public string Title { get; set; }
        public string Content { get; set; }

        public string Image { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public bool BreakingNews { get; set; } = false;
        public bool FeaturedNews { get; set; } = false;
        public bool IsPublished { get; set; } = false;







    }
}

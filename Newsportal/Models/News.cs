using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsportal.Models
{
    public class News
    {
        public int Id { get; set; }
        public int Count { get; set; } = 0;
        
        [Display(Name = "Published Date")]
        public DateTime PublishedDate { get; set; }
        public Category Category { get; set; }
        public Reporter Reporter { get; set; }
        
        [Display(Name = "Last Edited By")]
        public Reporter LastEditedBy { get; set; } = null;

        public string Title { get; set; }
        public string Content { get; set; }

        public string Image { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        
        [Display(Name = "Braking News")]
        public bool BreakingNews { get; set; } = false;
        
        [Display(Name = "Featured News")]
        public bool FeaturedNews { get; set; } = false;
        
        [Display(Name = "Is Published")]
        public bool IsPublished { get; set; } = false;







    }
}

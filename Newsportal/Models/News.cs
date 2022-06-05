using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsportal.Models
{
    public class News
    {
        protected News()
        {
            
        }
        public News(DateTime publishedDate, Reporter reporter, Category category)
        {
            PublishedDate = publishedDate;
            Reporter = reporter;
            Category = category;
        }
        
        public int Id { get; init; }
        public int Count { get; set; }
        
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
        public bool BreakingNews { get; set; }
        
        [Display(Name = "Featured News")]
        public bool FeaturedNews { get; set; }
        
        [Display(Name = "Is Published")]
        public bool IsPublished { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public decimal Rating { get; set; } = 0;
        
    }
}

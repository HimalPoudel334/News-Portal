using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        public decimal Rating
        {
            get
            {
                if (!Ratings.Any()) return 0;
                var newsRatings = Ratings.Where(r => r.NewsId == Id).ToList();
                var totalRatings = newsRatings.Sum(r => r.Rating);
                return totalRatings / newsRatings.Count;
            }
            set { }
        }
        public int TotalLikes
        {
            get
            {
                return !Likes.Any() ? 0 : Likes.Count(l => l.NewsId == Id);
            }
            set { }
        }

        //wondering whether all the records from ratings will come or just the one with this news
        public ICollection<NewsRating> Ratings { get; set; } = new List<NewsRating>(); 
        
        //wondering whether all the records from likes will come or just the one with this news
        public ICollection<NewsLikes> Likes { get; set; } = new List<NewsLikes>();

    }
}

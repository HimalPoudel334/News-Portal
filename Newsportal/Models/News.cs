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
        public DateTime PublishedDate { get; set; }
        public virtual Category Category { get; set; }
        public virtual Reporter Reporter { get; set; }
        public virtual Reporter LastEditedBy { get; set; } = null;
        public string Title { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public bool BreakingNews { get; set; }
        public bool FeaturedNews { get; set; }
        public bool IsPublished { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        
        [NotMapped]
        public decimal Rating {
            get
            {
                if (!Ratings.Any()) return 0;
                var newsRatings = Ratings.Where(r => r.NewsId == Id).ToList();
                var totalRatings = newsRatings.Sum(r => r.Rating);
                return totalRatings / newsRatings.Count;
            }

        }
        
        [NotMapped]
        public int TotalLikes => !Likes.Any() ? 0 : Likes.Count(l => l.NewsId == Id);
        
        [NotMapped] 
        public bool UserLikes { get; set; }

        //wondering whether all the records from ratings will come or just the one with this news
        public virtual ICollection<NewsRating> Ratings { get; set; } = new List<NewsRating>(); 
        
        //wondering whether all the records from likes will come or just the one with this news
        public virtual ICollection<NewsLikes> Likes { get; set; } = new List<NewsLikes>();

        public void SetUserLikes(string userId)
        {
            UserLikes = Likes.Any(likes => likes.NewsId == Id && likes.UserId == userId);
        }

    }
}

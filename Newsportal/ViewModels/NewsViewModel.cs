using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Newsportal.Models;

namespace Newsportal.ViewModels;

public class NewsCreateViewModel
{
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    [Display(Name = "Featured News")]
    public int? CategoryId { get; set; }
    
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    
    [Display(Name = "Featured News")]
    public bool IsFeatured { get; set; } = false;
    
    [Display(Name = "Braking News")]
    public bool IsBreaking { get; set; } = false;
    
    [Display(Name = "Published News")]
    public bool IsPublished { get; set; } = false;
    
    [Display(Name = "Image")]
    public IFormFile? ImageFile { get; set; }

}

public class NewsIndexViewModel : NewsCreateViewModel
{
    public int Count { get; set; }
    public decimal Rating { get; set; }
    public bool UserLikes { get; set; }
    public virtual ICollection<NewsRating> Ratings { get; set; } = new List<NewsRating>();
    public virtual ICollection<NewsLikes> Likes { get; set; } = new List<NewsLikes>();
    public int TotalLikes { get; set; }

}

public class NewsUpdateViewModel : NewsCreateViewModel
{
    public int Id { get; set; }
    public string Image { get; set; }
    
    [Display(Name = "Published Date")]
    public DateTime PublishedDate { get; set; }

}
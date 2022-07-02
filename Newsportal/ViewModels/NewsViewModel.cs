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
    public bool IsFeatured { get; set; }
    
    [Display(Name = "Breaking News")]
    public bool IsBreaking { get; set; }
    
    [Display(Name = "Published News")]
    public bool IsPublished { get; set; }
    
    [Display(Name = "Image")]
    public IFormFile? ImageFile { get; set; }

}

public class NewsUpdateViewModel : NewsCreateViewModel
{
    public int Id { get; set; }
    public string Image { get; set; }
    
    [Display(Name = "Published Date")]
    public DateTime PublishedDate { get; set; }

}

public class RecommendedNewsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public decimal Rating { get; set; }
    public int CategoryId { get; set; }
}

public class DetailedNewsViewModel
{
    public News News { get; set; }
    
    public ICollection<RecommendedNewsViewModel> RecommendedNews { get; set; } = new List<RecommendedNewsViewModel>();
}
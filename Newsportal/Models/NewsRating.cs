using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsportal.Models;

public class NewsRating
{
    [ForeignKey("User")]
    public string UserId { get; set; }
    public User User { get; set; }
    
    [ForeignKey("News")]
    public int NewsId { get; set; }
    public News News { get; set; }

    public decimal Rating { get; set; }
    
}

/*
            */
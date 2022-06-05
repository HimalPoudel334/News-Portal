using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsportal.Models;


public class NewsLikes
{
    [ForeignKey("User")]
    public string UserId { get; set; }
    public User User { get; set; }

    [ForeignKey("News")]
    public int NewsId { get; set; }
    public News News { get; set; }
}
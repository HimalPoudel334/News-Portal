using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsportal.Models;

public class NewsRating
{
    [ForeignKey("User")]
    public string UserId { get; set; }
    public virtual User User { get; set; }
    
    [ForeignKey("News")]
    public int NewsId { get; set; }
    public virtual News News { get; set; }

    public decimal Rating { get; set; }
    
}

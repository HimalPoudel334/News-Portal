using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsportal.Models;


public class NewsLikes
{
    protected NewsLikes()
    {
        
    }
    public NewsLikes(User user, News news)
    {
        User = user;
        UserId = user.Id;
        
        News = news;
        NewsId = news.Id;
    }
    [ForeignKey("User")]
    public string UserId { get; set; }
    public virtual User User { get; set; }

    [ForeignKey("News")]
    public int NewsId { get; set; }
    public virtual News News { get; set; }
}
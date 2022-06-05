namespace Newsportal.Models;

public class UserLikes
{
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int NewsId { get; set; }
    public News News { get; set; }
}
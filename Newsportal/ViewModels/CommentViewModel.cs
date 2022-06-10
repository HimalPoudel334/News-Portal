using System.ComponentModel.DataAnnotations;

namespace Newsportal.ViewModels;

public class CommentViewModel
{
    [Required]
    public int NewsId { get; set; }

    [Required]
    public string CommentContent { get; set; }
    
    public int? CommentId { get; set; }
}
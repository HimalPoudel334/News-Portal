using System.ComponentModel.DataAnnotations;

namespace Newsportal.ViewModels;

public class RatingViewModel
{
    [Required] public int NewsId { get; set; }
    [Required] public decimal Rating { get; set; }
    
}
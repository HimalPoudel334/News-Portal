using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Newsportal.Models;

public class User : IdentityUser
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
        
    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
        
    [Required]
    public string Address { get; set; }

    public UserType UserType { get; set; } = UserType.Subscriber;
}

public enum UserType
{
    Admin,
    Editor,
    Reporter,
    Subscriber
}
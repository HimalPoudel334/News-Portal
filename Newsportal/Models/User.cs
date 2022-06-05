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

    public string? Image { get; set; }
}

public class Reporter : User
{
    [Required]
    [Display(Name = "License Number")]
    public string LicenseNumber { get; set; }
        
}

public class Editor : User
{
    [Required]
    [Display(Name = "License Number")]
    public string LicenseNumber { get; set; }
        
}

public class Admin : User
{
    [Required]
    [Display(Name = "License Number")]
    public string LicenseNumber { get; set; }
        
}

public enum UserType
{
    Admin,
    Editor,
    Reporter,
    Subscriber
}
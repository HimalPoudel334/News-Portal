using System.ComponentModel.DataAnnotations;
using Newsportal.Models;

namespace Newsportal.ViewModels;

public class UserRegisterViewModel
{
    [Required]
    [EmailAddress] [RegularExpression("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-.]+$", ErrorMessage = "Enter a valid email address")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; }
    
    [Required]
    [Display(Name = "License Number")]

    public string LicenseNumber { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
        
    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
        
    [Required]
    public string Address { get; set; }
            
    [Display(Name = "User Type")]
    public UserType UserType { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }


    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
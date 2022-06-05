using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newsportal.Models
{
    public class Reporter : User
    {
        [Required]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; }
        
    }



}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Newsportal.Models
{
    public class Reporter : IdentityUser
    {
        [Required]
        public string LisenceNumber { get; set; }
        [Required]

        public string FirstName { get; set; }
        [Required]

        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
    }



}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Newsportal.ViewModels
{
    public class CreateRoleViewModel
        {
            [Required]
            [Display(Name = "Role")]
            public string RoleName { get; set; }
        }
    }

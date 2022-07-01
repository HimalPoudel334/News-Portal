using System;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newsportal.Data;
using Newsportal.Models;
using Newsportal.ViewModels;
using NuGet.Common;

namespace Newsportal.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(ILogger<AccountController>  logger, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.Cast<User>().ToListAsync(); 
        return View(users);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register([Bind("Email,PhoneNumber,LicenseNumber,FirstName,LastName,Address,UserType,Password,ConfirmPassword")] UserRegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        IdentityResult result = null;
        switch (model.UserType)
        {
            case UserType.Admin:
                Admin admin = new Admin()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    LicenseNumber = model.LicenseNumber,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    UserType = UserType.Admin
                };
                result = await _userManager.CreateAsync(admin, model.Password);
                break;
            case UserType.Editor:
                Editor editor = new Editor()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    LicenseNumber = model.LicenseNumber,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    UserType = UserType.Editor
                };
                result = await _userManager.CreateAsync(editor, model.Password);
                break;
            case UserType.Reporter:
                Reporter reporter = new Reporter()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    LicenseNumber = model.LicenseNumber,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    UserType = UserType.Reporter
                };
                result = await _userManager.CreateAsync(reporter, model.Password);
                break;
            default:
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    UserType = UserType.Subscriber
                };
                result = await _userManager.CreateAsync(user, model.Password);
                break;
        }

        
        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");
            
            //add user to role
            var roles = await _roleManager.Roles.ToListAsync();
            IdentityRole role = model.UserType switch
            {
                UserType.Admin => roles.FirstOrDefault(r => r.Name == "Admin"),
                UserType.Editor => roles.FirstOrDefault(r => r.Name == "Editor"),
                UserType.Reporter => roles.FirstOrDefault(r => r.Name == "Reporter"),
                _ => null
            };

            if (role != null)
            {
                //await _userManager.AddToRoleAsync(user, role.Name);
            }
            return RedirectToAction(nameof(Index));
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }
    
}
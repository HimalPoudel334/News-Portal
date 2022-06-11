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
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(ILogger<AccountController>  logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
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
    public async Task<IActionResult> Register([Bind("Email,PhoneNumber,LicenseNumber,FirstName,LastName,UserType,Password")] UserRegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Please enter required fields");
            return View(model);
        }

        User user = model.UserType switch
        {
            UserType.Reporter => new Reporter
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                LicenseNumber = model.LicenseNumber,
                Address = model.Address,
                UserType = UserType.Reporter
            },
            UserType.Editor => new Editor
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                LicenseNumber = model.LicenseNumber,
                Address = model.Address,
                UserType = UserType.Editor
            },
            UserType.Admin => new Admin
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                LicenseNumber = model.LicenseNumber,
                Address = model.Address,
                UserType = UserType.Admin
            },
            _ => new User
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                UserType = UserType.Subscriber
            }
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("User created a new account with password.");

            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(nameof(Index));
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return View(model);
    }
    
}
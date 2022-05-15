using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newsportal.Data;
using Newsportal.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Newsportal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;

        }


        public async Task<IActionResult> IndexAsync()
        {
            var user = (Reporter)await _userManager.GetUserAsync(this.User);


            //  return View(await _context.News.Include(n => n.Reporter).Include(n => n.Category).ToListAsync());
            // return View("~/Views/News/Index.cshtml", await _context.News.Include(n => n.Reporter).Include(n => n.Category).ToListAsync());
            ViewBag.CategoriesList = await _context.Category.ToListAsync();
            return View(await _context.News.Include(n => n.Reporter).Include(n => n.Category).Where(n => n.IsPublished).ToListAsync());
        }

        public IActionResult Privacy()
        {

            return View();
        }
        public async Task<IActionResult> DisplayAsync(string category)
        {

            return View(await _context.News.Include(n => n.Reporter).Include(n => n.Category).Where(n => n.Category.CategoryName == category).ToListAsync());
        }
        public async Task<IActionResult> DetailsNewsAsync(int id)
        {
            var detailedNews = await _context.News.Include(n => n.Reporter).Include(n => n.Category).FirstOrDefaultAsync(n => n.Id == id);
            if (!detailedNews.IsPublished)
            {
                return Content("Why do you wish to manipulate through url?");
            }
            detailedNews.Count++;
            await _context.SaveChangesAsync();



            return View(detailedNews);
        }
        public string ShowUserType()
        {
            return this.User.IsInRole("Admin") ? "Admin" : "Hi";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}

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
            var newsList = await _context.News.ToListAsync();
            
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                foreach (var news in newsList)
                {
                    if (news.Likes.Any(n => n.UserId == user.Id))
                    {
                        news.UserLikes = true;
                    }
                }    
            }

            ViewBag.CategoriesList = await _context.Category.ToListAsync();
            return View(newsList);
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
            var detailedNews = await _context.News
                .Include(n => n.Comments.Where(c => c.CommentId == null))
                .FirstOrDefaultAsync(n => n.Id == id);
            
            
            if (detailedNews == null)
            {
                return NotFound();
            }
            
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (detailedNews.Likes.Any(n => n.UserId == user.Id))
                {
                    detailedNews.UserLikes = true;
                }
            }
            
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

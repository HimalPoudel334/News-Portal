using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newsportal.Data;
using Newsportal.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using MlProject.DataModels;
using Newsportal.ViewModels;

namespace Newsportal.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<NewsController> _logger;
        private readonly PredictionEnginePool<NewsModel, CategoryPrediction> _predictionEnginePool;

        public NewsController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment hostEnvironment,
            ILogger<NewsController> logger,
            PredictionEnginePool<NewsModel,CategoryPrediction> predictionEnginePool)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = hostEnvironment;
            _logger = logger;
            _predictionEnginePool = predictionEnginePool;
        }

        // GET: News
        public async Task<IActionResult> Index()
        {
            var user = (Reporter)await _userManager.GetUserAsync(this.User);
            if (User.IsInRole("Admin"))
                return View(await _context.News.ToListAsync());
            return View(await _context.News.Where(a => a.Reporter.Id == user.Id).ToListAsync());
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create
        public async Task<IActionResult> Create()
        {
            var newsCreateViewModel = new NewsCreateViewModel()
            {
                Categories = await _context.Category.ToListAsync()
            };
            //ViewBag.Categories = new SelectList(await _context.Category.ToListAsync(), "Id", "CategoryName");
            return View(newsCreateViewModel);
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("CategoryId,Title,Content,ImageFile,BreakingNews,FeaturedNews,IsPublished")] NewsCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var reporter = (Reporter)await _userManager.GetUserAsync(this.User);
                Category category = null;
            
                //if Selected category is null, then predict the category of news
                if (model.CategoryId == null)
                {
                    var input = new NewsModel()
                    {
                        Content = model.Content,
                        Heading = model.Title
                    };
                    //here comes the prediction
                    var prediction = _predictionEnginePool.Predict(modelName: "CategoryClassifier", example: input);
                    var predictedCategory = prediction.PredictedLabel;
                    category = await _context.Category.FirstOrDefaultAsync(c => c.CategoryName.ToLower() == predictedCategory.ToLower());
                    if (category == null)
                    {
                        ModelState.AddModelError(string.Empty, "Category could not be predicted. Please enter category");
                        model.Categories = await _context.Category.ToListAsync();
                        return View(model);
                    }

                }
                else
                {
                    category = await _context.Category.FindAsync(model.CategoryId);
                }

                var news = new News(DateTime.Now, reporter, category)
                {
                    Title = model.Title,
                    Content = model.Content,
                    Image = UploadedFile(model)

                };

                _context.News.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e.Message);
                return BadRequest("Something went wrong");
            }
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var news = await _context.News.FindAsync(id.Value);
            var user = (Reporter) await _userManager.GetUserAsync(this.User);
            if (news == null || news.Reporter.Id != user.Id || news.LastEditedBy != null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Admin") && news.IsPublished)
            {
                ModelState.AddModelError(string.Empty, "Cannot edit Published news");
                return BadRequest("Cannot edit Published news");
            }

            var newsUpdateViewModel = new NewsUpdateViewModel()
            {
                Id = news.Id,
                Image = news.Image,
                PublishedDate = news.PublishedDate,
                Categories = await _context.Category.ToListAsync(),
                Content = news.Content,
                Title = news.Title,
                CategoryId = news.Category.Id,
                IsBreaking = news.BreakingNews,
                IsFeatured = news.FeaturedNews,
                IsPublished = news.IsPublished
            };

            return View(newsUpdateViewModel);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,PublishedDate,Title,Content,ImageFile,IsBreaking,IsBreaking,IsPublished")] NewsUpdateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                Category category = null;
                if (model.CategoryId == null)
                {
                    var input = new NewsModel()
                    {
                        Content = model.Content,
                        Heading = model.Title
                    };
                    //here comes the prediction
                    var prediction = _predictionEnginePool.Predict(modelName: "CategoryClassifier", example: input);
                    var predictedCategory = prediction.PredictedLabel;
                    category = await _context.Category.FirstOrDefaultAsync(c =>
                        c.CategoryName.ToLower() == predictedCategory.ToLower());
                    if (category == null)
                    {
                        ModelState.AddModelError(string.Empty, "Category could not be predicted. Please enter category");
                        model.Categories = await _context.Category.ToListAsync();
                        return View(model);
                    }

                }
                else
                {
                    category = await _context.Category.FindAsync(model.CategoryId);
                }

                var news = await _context.News.FindAsync(id);
                if (news == null) //this shouldn't happen
                {
                    return NotFound();
                }

                news.Category = category;
                news.PublishedDate = model.PublishedDate;
                news.Content = model.Content;
                news.Title = model.Title;
                news.IsPublished = model.IsPublished;
                news.FeaturedNews = model.IsFeatured;
                news.BreakingNews = model.IsBreaking;

                if (model.ImageFile != null)
                {
                    if (news.Image != null)
                    {
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/news/", news.Image);
                        System.IO.File.Delete(filePath);
                    }

                    news.Image = UploadedFile(model);
                }

                _context.Update(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!NewsExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                
            }
            return BadRequest("Something Went Wrong!");
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return Content("No Content Found to delete");
            }
            if (!User.IsInRole("Admin") && news.IsPublished)
                return Content("cannot delete published news");

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            var newsImage = Path.Combine(_webHostEnvironment.WebRootPath, "images/news/", news.Image);
            if (System.IO.File.Exists(newsImage))
            {
                System.IO.File.Delete(newsImage);
            }
            
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }

        private string UploadedFile(NewsCreateViewModel model)
        {
            string uniqueFileName = null;

            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/news");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LikeNews(int newsId)
        {
            var user = await _userManager.GetUserAsync(User);
            var news = await _context.News.FindAsync(newsId) ?? throw new Exception("News Not found");
            var isLiked = true;
            try
            {
                var newsLike = await _context.NewsLikes.FirstOrDefaultAsync(n => n.NewsId == news.Id && n.UserId == user.Id);
                if (newsLike != null)
                {
                    isLiked = false;
                    _context.NewsLikes.Remove(newsLike);
                }
                else
                {
                    newsLike = new NewsLikes((User) user, news);
                    _context.NewsLikes.Add(newsLike);
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("DbUpdateException ho yo " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Generic exception ho yo " + ex.Message);

            }
            
            return Ok(new { news.TotalLikes, isLiked });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentViewModel model)
        {
            var user = (User) await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("Listen here you dumb fuck! Don't you have some common sense. You need to login to comment");
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest("Listen here you dumb fuck! You need to provide correct data to comment");
            }

            var news = await _context.News.FindAsync(model.NewsId);
            if (news == null)
            {
                return BadRequest("Cannot find news. Looks like you did something fishy you little shit!");
            }

            Comment actualComment = null;
            if (model.CommentId != null)
            {
                actualComment = await _context.Comments.FindAsync(model.CommentId);
            }

            var comment = new Comment()
            {
                Content = model.CommentContent,
                News = news,
                CommentId = actualComment?.Id,
                CommentedBy = user,
                CommentedOn = DateTime.Now
            };
            
            _context.Comments.Add(comment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }

            var reply = new
            {
                CommentedBy = $"{comment.CommentedBy.FirstName} {comment.CommentedBy.FirstName}",
                CommentedOn = comment.CommentedOn.ToString("g"),
                comment.Content,
                comment.Id
            };
            
            return Ok(reply);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRating(RatingViewModel model)
        {
            var user = (User) await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest("Listen here you dumb fuck! Don't you have some common sense. You need to login to comment");
            }
            
            if (!ModelState.IsValid)
            {
                return BadRequest("Listen here you little piece of shit! Give rating properly else I will fuck your mom!");
            }
            
            var news = await _context.News.FindAsync(model.NewsId);
            if (news == null)
            {
                return BadRequest("Cannot find news. Looks like you did something fishy you little shit!");
            }

            
            var rating = await _context.NewsRatings.FirstOrDefaultAsync(r => r.NewsId == news.Id && r.UserId == user.Id);
            if (rating != null)
            {
                rating.Rating = model.Rating;
                _context.NewsRatings.Update(rating);
            }
            else
            {
                rating = new NewsRating()
                {
                    News = news,
                    NewsId = news.Id,
                    User = user,
                    UserId = user.Id,
                    Rating = model.Rating
                };
                _context.NewsRatings.Add(rating);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Oops! Something went wrong while updating rating");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Oops! Something went wrong while saving rating");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Oops! Something went wrong while saving rating");
            }

            return Ok(rating.Rating);

        }
    }
    
    //implicit operator
    //        public static implicit operator BalanceType?(string name) => GetInstanceByName<BalanceType>(name);

}

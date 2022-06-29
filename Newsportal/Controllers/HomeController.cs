﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newsportal.Data;
using Newsportal.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.ML;
using MlProject.DataModels;
using Newsportal.ViewModels;

namespace Newsportal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PredictionEnginePool<NewsRatingDataModel, NewsRatingPrediction> _predictionEnginePool;
        private readonly ApplicationDbContext _context;


        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            PredictionEnginePool<NewsRatingDataModel,NewsRatingPrediction> predictionEnginePool)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _predictionEnginePool = predictionEnginePool;
        }


        public async Task<IActionResult> IndexAsync()
        {
            var newsList = await _context.News.Where(n => n.IsPublished).ToListAsync();
            
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                newsList.ForEach(n => n.SetUserLikes(user.Id));   
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
            var user = await _userManager.GetUserAsync(User);
            var news = await _context.News
                .Where(n => n.Category.CategoryName.ToLower() == category.Trim().ToLower())
                .ToListAsync();
            
            if (user != null)
            {
                news.ForEach(n => n.SetUserLikes(user.Id));
            }
            
            return View(news);
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
                detailedNews.SetUserLikes(user.Id);
            }

            var allNews = await _context.News.Where(n => n.Id != detailedNews.Id && n.IsPublished).Select(n => new RecommendedNewsViewModel()
            {
                Id = n.Id,
                Title = n.Title,
                Image = n.Image,
                Rating = n.Rating,
                CategoryId = n.Category.Id
            }).ToListAsync();
            var allUsers = await _userManager.Users.Select(x => x.Id).ToListAsync();
            var userIndex = allUsers.IndexOf(user.Id) + 1;
            var recommendedNews = new List<RecommendedNewsViewModel>();
            foreach (var news in allNews)
            {
                var newsRatingInput = new NewsRatingDataModel()
                {
                    Rating = (float) detailedNews.Rating,
                    CategoryId = detailedNews.Category.Id,
                    UserId = (uint) userIndex,
                    NewsId = news.Id,
                };
            
                //here comes the recommendation
                var prediction = _predictionEnginePool.Predict(modelName: "NewsRecommender", example: newsRatingInput);
                if (prediction.Score >= 4)
                {
                    recommendedNews.Add(news);
                }

            }            
            
            if (!detailedNews.IsPublished)
            {
                return Content("Why do you wish to manipulate through url?");
            }
            detailedNews.Count++;
            await _context.SaveChangesAsync();

            var detailedNewsViewModel = new DetailedNewsViewModel()
            {
                News = detailedNews,
                RecommendedNews = recommendedNews
            };

            return View(detailedNewsViewModel);
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

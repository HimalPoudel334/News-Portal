﻿using Microsoft.AspNetCore.Authorization;
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

namespace Newsportal.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public NewsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            webHostEnvironment = hostEnvironment;

        }

        // GET: News
        public async Task<IActionResult> Index()
        {
            var user = (Reporter)await _userManager.GetUserAsync(this.User);
            if (User.IsInRole("Admin"))
                return View(await _context.News.Include(n => n.Reporter).Include(n => n.Category).ToListAsync());
            return View(await _context.News.Include(n => n.Reporter).Include(n => n.Category).Where(a => a.Reporter.Id == user.Id).ToListAsync());
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
            ViewBag.Categories = new SelectList(await _context.Category.ToListAsync(), "Id", "CategoryName");
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Category,Title,Content,ImageFile,BreakingNews,FeaturedNews,IsPublished")] News news)
        {
            //var errors = ModelState.Values.SelectMany(v => v.Errors).ElementAt(0);
            news.Category = _context.Category.Where(a => a.Id == news.Category.Id).Single();

            // errors = ModelState.Values.SelectMany(v => v.Errors).ElementAt(0);



            if (!ModelState.IsValid)
            {
                var user = (Reporter)await _userManager.GetUserAsync(this.User);
                news.Reporter = user;
                news.PublishedDate = DateTime.Now;
                news.Image = UploadedFile(news);

                //from cust in customers
                //where cust.City == "London"

                // news.Category =  from cat in await _context.Category.FindAsync(news.Category) where cat.Id== news.Category.ID select news.Category ;

                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _context.Category.ToListAsync(), "Id", "CategoryName");
            return View("create", news);
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(await _context.Category.ToListAsync(), "Id", "CategoryName");


            var news = await _context.News.Include(i => i.Reporter).Include(i => i.LastEditedBy).Include(i => i.Category).FirstOrDefaultAsync(i => i.Id == id.Value);
            var user = await _userManager.GetUserAsync(this.User);
            /*if (news.IsPublished) { return Content("Cannot edit Published news"); }*/
            //await db.Items.Include(i => i.ItemVerifications).FirstOrDefaultAsync(i => i.Id == id.Value);

            if (news == null || news.Reporter.Id != user.Id || news.LastEditedBy != null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Count,Category,PublishedDate,Title,Content,Image,BreakingNews,FeaturedNews,IsPublished")] News news)
        {
            if (id != news.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                try
                {
                    news.Category = _context.Category.Where(a => a.Id == news.Category.Id).Single();

                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(news);
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
                return Content("cannot delelte published news");

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }

        private string UploadedFile(News model)
        {
            string uniqueFileName = null;

            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(fileStream);
                }
            }
            return "/images/" + uniqueFileName;
        }
    }
}

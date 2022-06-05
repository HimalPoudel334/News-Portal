using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newsportal.Models;

namespace Newsportal.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NewsLikes>()
                .HasKey(c => new { c.UserId, c.NewsId });
            
            modelBuilder.Entity<NewsRating>()
                .HasKey(c => new { c.UserId, c.NewsId });
            
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Newsportal.Models.News> News { get; set; }
        public DbSet<Newsportal.Models.Category> Category { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<NewsRating> NewsRatings { get; set; }
        public DbSet<NewsLikes> NewsLikes { get; set; }
    }
    
}

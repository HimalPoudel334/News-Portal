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
        public DbSet<Newsportal.Models.News> News { get; set; }
        public DbSet<Newsportal.Models.Category> Category { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}

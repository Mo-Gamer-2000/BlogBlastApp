/*
 * This class defines the ApplicationDbContext, which represents the database context for the BlogBlast application.
 */

using BlogBlast.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } // Represents the categories table in the database
        public DbSet<Post> BlogPosts { get; set; } // Represents the blog posts table in the database
        public DbSet<Subscription> Subscriptions { get; set; } // Represents the subscriptions table in the database
    }
}
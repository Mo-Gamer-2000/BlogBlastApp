using BlogBlast.Components.Pages;
using BlogBlast.Data;
using BlogBlast.Data.Entities;
using BlogBlast.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace BlogBlast.Services;

// Defined an interface for managing blog posts in the admin panel
public interface IPostAdminService
{
    // Method to fetch a paginated list of blog posts
    Task<PagedResult<Post>> GetBlogPostsAsync(int startIndex, int pageSize);

    // Method to fetch blog posts by their IDs
    Task<Post?> GetBlogPostsByIdAsync(int id);

    // Method to save a blog post
    Task<Post> SaveBlogPostAsync(Post blogPost);
}

// Implementation of the blog post admin service interface
public class PostAdminService : IPostAdminService
{
    // Dependency injection for the database context factory
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    // Constructor to initialise the service with the context factory
    public PostAdminService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // Template method to execute database queries
    private async Task<TResult> ExecuteOnContext<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
    {
        // Create a database context
        using var context = _contextFactory.CreateDbContext();
        // Execute the query and return the result
        return await query.Invoke(context);
    }

    // Method to retrieve a paginated list of blog posts
    public async Task<PagedResult<Post>> GetBlogPostsAsync(int startIndex, int pageSize)
    {
        return await ExecuteOnContext(async context =>
        {
            // Query the blog posts from the database
            var query = context.BlogPosts.AsNoTracking();
            var count = await query.CountAsync();
            var records = await query.Include(b => b.Category)
                                .OrderByDescending(b => b.Id)
                                .Skip(startIndex)
                                .Take(pageSize)
                                .ToArrayAsync();

            // Return the paginated result
            return new PagedResult<Post>(records, count);
        });
    }

    // Method to fetch blog posts by their IDs
    public async Task<Post?> GetBlogPostsByIdAsync(int id) =>
        await ExecuteOnContext(async context =>
            await context.BlogPosts
                .AsNoTracking()
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id)
        );

    // Method to save a blog post (not implemented)
    public Task<Post> SaveBlogPostAsync(Post blogPost)
    {
        throw new NotImplementedException();
    }
}

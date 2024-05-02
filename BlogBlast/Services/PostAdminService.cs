using BlogBlast.Data;
using BlogBlast.Data.Entities;
using BlogBlast.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Services;

// Defined an interface for managing blog posts in the admin panel
public interface IPostAdminService
{
    // Method to fetch a paginated list of blog posts
    Task<PagedResult<Post>> GetBlogPostsAsync(int startIndex, int pageSize);

    // Method to fetch blog posts by their IDs
    Task<Post?> GetBlogPostsByIdAsync(int id);

    // Method to save a blog post
    Task<Post> SaveBlogPostAsync(Post blogPost, string userId);
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

    // Method to Generate a unique slug for a blog post based on its title
    private async Task<string> GenerateSlugAsync(Post blogPost)
    {
        // Generate a unique slug for the blog post
        return await ExecuteOnContext(async context =>
        {
            string originalSlug = blogPost.Title.ToSlug();
            string slug = originalSlug;
            int count = 1;
            // Check if the generated slug already exists, if yes, append a count to it
            while (await context.BlogPosts.AsNoTracking().AnyAsync(b => b.Slug == slug))
            {
                slug = $"{originalSlug}-{count++}";
            }
            return slug;
        });
    }

    // Method to save a blog post
    public Task<Post> SaveBlogPostAsync(Post blogPost, string userId)
    {
        // Save a blog post to the database
        return ExecuteOnContext(async context =>
        {
            if (blogPost.Id == 0)
            {
                // New Blog Post
                var isDuplicateTitle = await context.BlogPosts
                                        .AsNoTracking()
                                        .AnyAsync(b => b.Title == blogPost.Title);
                if (isDuplicateTitle)
                {
                    throw new InvalidOperationException("Blog Post with this Name Already Exists!");
                }

                // Generate and set slug
                blogPost.Slug = await GenerateSlugAsync(blogPost);
                blogPost.CreatedAt = DateTime.UtcNow;
                blogPost.UserId = userId;

                // Set published date if post is published
                if (blogPost.IsPublished)
                {
                    blogPost.PublishedAt = DateTime.UtcNow;
                }
                await context.BlogPosts.AddAsync(blogPost);
            }
            else
            {
                // Existing Blog Post
                var isDuplicateTitle = await context.BlogPosts
                                        .AsNoTracking()
                                        .AnyAsync(b => b.Title == blogPost.Title && b.Id != blogPost.Id);
                if (isDuplicateTitle)
                {
                    throw new InvalidOperationException("Blog Post with this Title Already Exists!");
                }

                var dbBlog = await context.BlogPosts.FindAsync(blogPost.Id);

                // Update blog post properties
                dbBlog.Title = blogPost.Title;
                dbBlog.Image = blogPost.Image;
                dbBlog.Introduction = blogPost.Introduction;
                dbBlog.Content = blogPost.Content;
                dbBlog.CategoryId = blogPost.CategoryId;
                dbBlog.IsPublished = blogPost.IsPublished;
                dbBlog.IsFeatured = blogPost.IsFeatured;

                // Update published date if post is published
                if (blogPost.IsPublished)
                {
                    if (!dbBlog.IsPublished)
                    {
                        blogPost.PublishedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    blogPost.PublishedAt = null;
                }
            }

            await context.SaveChangesAsync();
            return blogPost;
        });
    }
}
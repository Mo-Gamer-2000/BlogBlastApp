/*
 * This class defines the PostAdminService, which provides methods for managing blog posts in the admin panel.
 */

using BlogBlast.Data;
using BlogBlast.Data.Entities;
using BlogBlast.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Services
{
    // Represents the blog post admin service interface
    public interface IPostAdminService
    {
        Task<PagedResult<Post>> GetBlogPostsAsync(int startIndex, int pageSize); // Method to fetch a paginated list of blog posts
        Task<Post?> GetBlogPostsByIdAsync(int id); // Method to fetch blog posts by their IDs
        Task<Post> SaveBlogPostAsync(Post blogPost, string userId); // Method to save a blog post
    }

    // Represents the blog post admin service implementation
    public class PostAdminService : IPostAdminService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory; // Factory for creating database contexts

        public PostAdminService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory; // Initializes the context factory
        }

        // Template method to execute database queries
        private async Task<TResult> ExecuteOnContext<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
        {
            // Creates a new database context
            using var context = _contextFactory.CreateDbContext();
            // Invokes the query function with the context and returns the result
            return await query.Invoke(context);
        }

        // Method to retrieve a paginated list of blog posts
        public async Task<PagedResult<Post>> GetBlogPostsAsync(int startIndex, int pageSize)
        {
            // Executes the query on the context and returns the paged result
            return await ExecuteOnContext(async context =>
            {
                var query = context.BlogPosts.AsNoTracking();
                var count = await query.CountAsync();
                var records = await query.Include(b => b.Category)
                                    .OrderByDescending(b => b.Id)
                                    .Skip(startIndex)
                                    .Take(pageSize)
                                    .ToArrayAsync();
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

        // Method to generate a unique slug for a blog post based on its title
        private async Task<string> GenerateSlugAsync(Post blogPost)
        {
            // Generates a unique slug for the blog post
            return await ExecuteOnContext(async context =>
            {
                string originalSlug = blogPost.Title.ToSlug();
                string slug = originalSlug;
                int count = 1;
                // Checks if the generated slug already exists, if yes, appends a count to it
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
            // Saves a blog post to the database
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

                    // Generates and sets slug
                    blogPost.Slug = await GenerateSlugAsync(blogPost);
                    blogPost.CreatedAt = DateTime.UtcNow;
                    blogPost.UserId = userId;

                    // Sets published date if post is published
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

                    // Updates blog post properties
                    dbBlog.Title = blogPost.Title;
                    dbBlog.Image = blogPost.Image;
                    dbBlog.Introduction = blogPost.Introduction;
                    dbBlog.Content = blogPost.Content;
                    dbBlog.CategoryId = blogPost.CategoryId;
                    dbBlog.IsPublished = blogPost.IsPublished;
                    dbBlog.IsFeatured = blogPost.IsFeatured;

                    // Updates published date if post is published
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
}
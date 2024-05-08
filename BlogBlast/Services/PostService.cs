/*
 * This class defines the PostService, which provides methods for retrieving and managing blog posts.
 */

using BlogBlast.Data;
using BlogBlast.Data.Entities;
using BlogBlast.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Services
{
    // Represents the interface for managing posts
    public interface IPostService
    {
        Task<Post[]> GetFeaturedPostsAsync(int count, int categoryId = 0); // Retrieves featured posts asynchronously
        Task<Post[]> GetLatestPostsAsync(int count, int categoryId = 0); // Retrieves latest posts asynchronously
        Task<Post[]> GetPopularPostsAsync(int count, int categoryId = 0); // Retrieves popular posts asynchronously
        Task<DetailPageModel> GetPostBySlugAsync(string slug); // Retrieves a post by its slug asynchronously
        Task<Post[]> GetAllPostsAsync(int pageIndex, int pageSize, int categoryId = 0); // Retrieves all posts asynchronously with pagination and optional category filtering
    }

    // Represents the service class for managing posts
    public class PostService : IPostService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory; // Factory for creating database contexts

        public PostService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory; // Initializes the context factory
        }

        // Executes a query on the DbContext asynchronously
        private async Task<TResult> QueryOnContextAsync<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
        {
            // Creates a new database context
            using var context = _contextFactory.CreateDbContext();
            // Invokes the query function with the context and returns the result
            return await query(context);
        }

        // Retrieves featured posts asynchronously
        public async Task<Post[]> GetFeaturedPostsAsync(int count, int categoryId = 0)
        {
            // Executes the query asynchronously within the context and returns the result
            return await QueryOnContextAsync(async context =>
            {
                // Construct the initial query to retrieve featured posts
                var query = context.BlogPosts
                                   .AsNoTracking()
                                   .Include(p => p.Category) // Include related category information
                                   .Include(b => b.User)     // Include related user information
                                   .Where(b => b.IsPublished); // Filter by published status

                // If a categoryId is specified, filter the query by that category
                if (categoryId > 0)
                {
                    query = query.Where(b => b.CategoryId == categoryId);
                }

                // Order the results randomly and take the specified number of posts
                var records = await query
                                    .Where(b => b.IsFeatured)
                                    .OrderBy(_ => Guid.NewGuid())
                                    .Take(count)
                                    .ToArrayAsync();

                // Check if there are not enough featured posts, then include additional non-featured posts
                if (count < records.Length)
                {
                    var additionalRecords = await query
                                    .Where(b => !b.IsFeatured)
                                    .OrderBy(_ => Guid.NewGuid())
                                    .Take(count - records.Length)
                                    .ToArrayAsync();

                    // Concatenate the additional records with the existing featured records
                    records = records.Concat(additionalRecords).ToArray();
                }

                return records;
            });
        }

        // Retrieves popular posts asynchronously
        public async Task<Post[]> GetPopularPostsAsync(int count, int categoryId = 0)
        {
            // Executes the query asynchronously within the context and returns the result
            return await QueryOnContextAsync(async context =>
            {
                // Construct the initial query to retrieve popular posts
                var query = context.BlogPosts
                                   .AsNoTracking()
                                   .Include(p => p.Category) // Include related category information
                                   .Include(b => b.User)     // Include related user information
                                   .Where(b => b.IsPublished); // Filter by published status

                // If a categoryId is specified, filter the query by that category
                if (categoryId > 0)
                {
                    query = query.Where(b => b.CategoryId == categoryId);
                }

                // Order the results by descending (From Most to Least - View Counts) and take the specified number of posts
                return await query.OrderByDescending(b => b.ViewCount)
                                  .Take(count)
                                  .ToArrayAsync();
            });
        }

        // Retrieves latest posts asynchronously
        public async Task<Post[]> GetLatestPostsAsync(int count, int categoryId = 0) =>
            await GetPostsAsync(0, count, categoryId);

        // Retrieves a post by its slug asynchronously
        public async Task<DetailPageModel> GetPostBySlugAsync(string slug)
        {
            // Executes the query asynchronously within the context and returns the result
            return await QueryOnContextAsync(async context =>
            {
                // Retrieve the blog post with the specified slug and that is published
                var blogPost = await context.BlogPosts
                                        .AsNoTracking()
                                        .Include(b => b.Category) // Include related category information
                                        .Include(b => b.User)     // Include related user information
                                        .FirstOrDefaultAsync(b => b.Slug == slug && b.IsPublished);

                // If no matching post is found, return an empty DetailPageModel
                if (blogPost is null)
                {
                    return DetailPageModel.Empty();
                }

                // Retrieve related posts from the same category, excluding the current post, and randomize the order
                var relatedPosts = await context.BlogPosts
                                              .AsNoTracking()
                                              .Include(b => b.Category) // Include related category information
                                              .Include(b => b.User)     // Include related user information
                                              .Where(b => b.CategoryId == blogPost.CategoryId && b.IsPublished && b.Id != blogPost.Id)
                                              .OrderBy(_ => Guid.NewGuid())
                                              .Take(4)
                                              .ToArrayAsync();

                // Create a DetailPageModel instance with the retrieved blog post and related posts
                return new DetailPageModel(blogPost, relatedPosts);
            });
        }

        // Retrieves all posts asynchronously with pagination and optional category filtering
        public async Task<Post[]> GetAllPostsAsync(int pageIndex, int pageSize, int categoryId = 0) =>
            await GetPostsAsync(pageIndex * pageSize, pageSize, categoryId);

        private async Task<Post[]> GetPostsAsync(int skip, int take, int categoryId)
        {
            // Executes the query asynchronously within the context and returns the result
            return await QueryOnContextAsync(async context =>
            {
                // Construct the initial query to retrieve posts
                var query = context.BlogPosts
                                   .AsNoTracking()
                                   .Include(p => p.Category) // Include related category information
                                   .Include(b => b.User)     // Include related user information
                                   .Where(b => b.IsPublished); // Filter by published status

                // If a categoryId is specified, filter the query by that category
                if (categoryId > 0)
                {
                    query = query.Where(b => b.CategoryId == categoryId);
                }

                // Order the results by descending (From Most Recent to Oldest) and take the specified number of posts
                return await query.OrderByDescending(b => b.PublishedAt)
                                    .Skip(skip)
                                    .Take(take)
                                    .ToArrayAsync();
            });
        }
    }
}
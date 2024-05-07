using BlogBlast.Data;
using BlogBlast.Data.Entities;
using BlogBlast.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Services
{
    /// <summary>
    /// Represents the interface for managing posts.
    /// </summary>
    public interface IPostService
    {
        /// <summary>
        /// Retrieves featured posts asynchronously.
        /// </summary>
        /// <param name="count">The number of featured posts to retrieve.</param>
        /// <param name="categoryId">The ID of the category (optional).</param>
        /// <returns>An array of featured posts.</returns>
        Task<Post[]> GetFeaturedPostsAsync(int count, int categoryId = 0);

        /// <summary>
        /// Retrieves latest posts asynchronously.
        /// </summary>
        /// <param name="count">The number of latest posts to retrieve.</param>
        /// <param name="categoryId">The ID of the category (optional).</param>
        /// <returns>An array of latest posts.</returns>
        Task<Post[]> GetLatestPostsAsync(int count, int categoryId = 0);

        /// <summary>
        /// Retrieves popular posts asynchronously.
        /// </summary>
        /// <param name="count">The number of popular posts to retrieve.</param>
        /// <param name="categoryId">The ID of the category (optional).</param>
        /// <returns>An array of popular posts.</returns>
        Task<Post[]> GetPopularPostsAsync(int count, int categoryId = 0);

        /// <summary>
        /// Retrieves a post by its slug asynchronously.
        /// </summary>
        /// <param name="slug">The slug of the post.</param>
        /// <returns>A detailed page model of the post.</returns>
        Task<DetailPageModel> GetPostBySlugAsync(string slug);
    }

    /// <summary>
    /// Service class for managing posts.
    /// </summary>
    public class PostService : IPostService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostService"/> class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating DbContext instances.</param>
        public PostService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Executes a query on the DbContext asynchronously.
        /// </summary>
        /// <typeparam name="TResult">The type of the query result.</typeparam>
        /// <param name="query">The function representing the query.</param>
        /// <returns>The result of the query.</returns>
        private async Task<TResult> QueryOnContextAsync<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
        {
            using var context = _contextFactory.CreateDbContext();
            return await query(context);
        }

        /// <summary>
        /// Retrieves featured posts asynchronously.
        /// </summary>
        /// <param name="count">The number of featured posts to retrieve.</param>
        /// <param name="categoryId">The ID of the category to filter by (optional).</param>
        /// <returns>An array of featured posts.</returns>
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

        /// <summary>
        /// Retrieves popular posts asynchronously.
        /// </summary>
        /// <param name="count">The number of popular posts to retrieve.</param>
        /// <param name="categoryId">The ID of the category (optional).</param>
        /// <returns>An array of popular posts.</returns>
        public async Task<Post[]> GetPopularPostsAsync(int count, int categoryId = 0)
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

                // Order the results by descending (From Most to Least - View Counts) and take the specified number of posts
                return await query.OrderByDescending(b => b.ViewCount)
                                  .Take(count)
                                  .ToArrayAsync();
            });
        }

        /// <summary>
        /// Retrieves latest posts asynchronously.
        /// </summary>
        /// <param name="count">The number of latest posts to retrieve.</param>
        /// <param name="categoryId">The ID of the category (optional).</param>
        /// <returns>An array of latest posts.</returns>
        public async Task<Post[]> GetLatestPostsAsync(int count, int categoryId = 0)
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

                // Order the results by publisehd date and take the specified number of posts
                return await query.OrderByDescending(b => b.PublishedAt)
                                  .Take(count)
                                  .ToArrayAsync();
            });
        }

        /// <summary>
        /// Retrieves a post by its slug asynchronously.
        /// </summary>
        /// <param name="slug">The slug of the post.</param>
        /// <returns>A detailed page model of the post.</returns>
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
    }
}
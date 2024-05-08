/*
 * This class defines the CategoryService, which provides methods for managing categories in the BlogBlast application.
 */

using BlogBlast.Data;
using BlogBlast.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Services
{
    // Represents the category service interface
    public interface ICategoryService
    {
        Task<Category[]> GetCategoriesAsync(); // Method to get all categories asynchronously
        Task<Category?> GetCategoryBySlugAsync(string slug); // Method to get a category by slug asynchronously
        Task<Category> SaveCategoryAsync(Category category); // Method to save a category asynchronously
    }

    // Represents the category service implementation
    public class CategoryService : ICategoryService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory; // Factory for creating database contexts

        public CategoryService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory; // Initializes the context factory
        }

        // Template method to execute operations on the database context
        private async Task<TResult> ExecuteOnContext<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
        {
            // Creates a new database context
            using var context = _contextFactory.CreateDbContext();
            // Invokes the query function with the context and returns the result
            return await query.Invoke(context);
        }

        // Method to get all categories asynchronously
        public async Task<Category[]> GetCategoriesAsync()
        {
            // Executes the query on the context and returns the categories
            return await ExecuteOnContext(async context =>
            {
                var categories = await context.Categories.AsNoTracking().ToArrayAsync(); // Retrieves categories from the database
                return categories;
            });
        }

        // Method to save a category asynchronously
        public async Task<Category> SaveCategoryAsync(Category category)
        {
            // Executes the query on the context and returns the saved category
            return await ExecuteOnContext(async context =>
            {
                if (category.Id == 0)
                {
                    if (await context.Categories.AsNoTracking().AnyAsync(c => c.Name == category.Name))
                    {
                        throw new InvalidOperationException($"Category with the name provided: {category.Name} already exists!");
                    }
                    category.Slug = category.Name.ToSlug();
                    await context.Categories.AddAsync(category);
                }
                else
                {
                    if (await context.Categories.AsNoTracking().AnyAsync(c => c.Name == category.Name && c.Id != category.Id))
                    {
                        throw new InvalidOperationException($"Category with the name provided: {category.Name} already exists!");
                    }
                    var dbCategory = await context.Categories.FindAsync(category.Id);
                    dbCategory.Name = category.Name;
                    dbCategory.VisibleOnNavbar = category.VisibleOnNavbar;
                    category.Slug = dbCategory.Slug;
                }
                await context.SaveChangesAsync();
                return category;
            });
        }

        // Method to get a category by slug asynchronously
        public async Task<Category?> GetCategoryBySlugAsync(string slug) =>
            await ExecuteOnContext(async context =>
                await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug)
            );
    }
}
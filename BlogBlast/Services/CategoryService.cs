using BlogBlast.Data;
using BlogBlast.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Services;
public class CategoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public CategoryService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // Create template method
    private async Task<TResult> ExecuteOnContext<TResult>(Func<ApplicationDbContext, Task<TResult>> query)
    {
        // Create a DB  context
        using var context = _contextFactory.CreateDbContext();
        // Return the result and dispose it
        return await query.Invoke(context);
    }

    // Get All Categories Method
    public async Task<Category[]> GetCategoriesAsync()
    {
        return await ExecuteOnContext(async context =>
        {
            // Dispose after code execution leaves the body of this method - Passsed to inner lapda function, then it will execute
            var categories = await context.Categories.AsNoTracking().ToArrayAsync();
            return categories;
        });
    }

    // Save Category Method
    public async Task<Category> SaveCategoryAsync(Category category)
    {
        return await ExecuteOnContext(async context =>
        {
            // Check for category exsitance
            if (category.Id == 0)
            {
                // If the category = 0, which means it is a new category
                //  Check if category exists in the DB
                if (await context.Categories.AsNoTracking().AnyAsync(c=> c.Name == category.Name))
                {
                    // If the category exists with that name, then throw an error
                    throw new InvalidOperationException($"Category with the name provided: {category.Name} already exists!");
                }
                // If the category does not exist, then we create it and save it
                // Create a slug for category
                category.Slug = category.Name.ToSlug();
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();
            }
            else
            {
                // Else,  //  Check if category exists in the DB
                if (await context.Categories.AsNoTracking().AnyAsync(c => c.Name == category.Name && c.Id != category.Id)) // Check the name with different ID, Not the same!
                {
                    // If the category exists with that name and different ID, then throw an error
                    throw new InvalidOperationException($"Category with the name provided: {category.Name} already exists!");
                }
                // Get category from DB, via id
                var dbCategory = await context.Categories.FindAsync(category.Id);

                // Check if the the names are equal
                dbCategory.Name = category.Name;
                dbCategory.VisibleOnNavbar = category.VisibleOnNavbar; // Then make it visible on the navbar
                category.Slug = dbCategory.Slug; // Checking if the slug are equal, if not revert back to db slug
            }
            await context.SaveChangesAsync(); // Save the chnages
            return category; // and return the category
        });
    }

    // Get category by slug method
    public async Task<Category?> GetCategoryBySlugAsync(string slug) => 
        await ExecuteOnContext(async context => 
            await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Slug == slug) // return category by slug
        );
}

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

    public async Task<Category[]> GetCategoriesAsync()
    {
        // Dispose after code execution leaves the body of this method
        using var context = _contextFactory.CreateDbContext();
        var categories = await context.Categories.AsNoTracking().ToArrayAsync();
        return categories;
    }
}

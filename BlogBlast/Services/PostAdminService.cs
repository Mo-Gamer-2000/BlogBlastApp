using BlogBlast.Components.Pages;
using BlogBlast.Data.Entities;

namespace BlogBlast.Services;

public interface IPostAdminService
{
    Task<Post[]> GetBlogPostsAsync();
    Task<Post[]> GetBlogPostsByIdAsync(int id); 
    Task<Post[]> SaveBlogPostAsync(Post blogPost); 
}

public class PostAdminService
{
    // Code Goes here
}

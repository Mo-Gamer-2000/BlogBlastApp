using BlogBlast.Data;
using BlogBlast.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Services;

internal static class AdminAccount
{
	public const string Name = "Moeez Abdul";
	public const string Email = "Moeez@gmail.com";
	public const string Role = "Admin";
	public const string Password = "Admin24680";
}
public class SeedService
{
	private readonly ApplicationDbContext _context;
	private readonly IUserStore<ApplicationUser> _userStore;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;

	public SeedService(ApplicationDbContext context,IUserStore<ApplicationUser> userStore, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
		_context = context;
		_userStore = userStore;
		_userManager = userManager;
		_roleManager = roleManager;
	}

    public async Task SeedDataAsync()
	{
        // Seed Admin Role
		// Check if this role (Admin) exists in our DB
        if (await _userManager.FindByNameAsync(AdminAccount.Role) is null)
        {
            // Check Admin role does not exist in the DB
			// Then, create one
			var adminRole = new IdentityRole(AdminAccount.Role);

			var result = await _roleManager.CreateAsync(adminRole);
			// Check if Admin was not created
            if (!result.Succeeded)
            {
				var errorMessage = result.Errors.Select(e => e.Description);
				throw new Exception($"An Error Occurred While Creating an Admin Role {Environment.NewLine}{string.Join(Environment.NewLine, errorMessage)}");
            }
        }

		// Seed Admin User

		var adminUser = await _userManager.FindByEmailAsync(AdminAccount.Email);
        if (adminUser is null)
        {
			// Check Admin user does not exist in the DB
			// Then, create one
			adminUser = new ApplicationUser();
			adminUser.Name = AdminAccount.Name;
			await _userStore.SetUserNameAsync(adminUser, AdminAccount.Email, CancellationToken.None);
			var result = await _userManager.CreateAsync(adminUser, AdminAccount.Password);

			// Check if Admin was not created
			if (!result.Succeeded)
			{
				var errorMessage = result.Errors.Select(e => e.Description);
				throw new Exception($"An Error Occurred While Creating an Admin User {Environment.NewLine}{string.Join(Environment.NewLine, errorMessage)}");
			}
		}

        // Seed Initial Categories
		// Check if there is not category
        if (!await _context.Categories.AsNoTracking().AnyAsync())
        {
			// Check if there is no categories in the DB
			// Then, create category
			await _context.Categories.AddRangeAsync(Category.GetSeedCategories());
			await _context.SaveChangesAsync();
		}
	}
}

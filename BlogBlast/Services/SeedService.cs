using BlogBlast.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

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
            // This Admin role does not exist in the DB
			// Then, create one
			var adminRole = new IdentityRole(AdminAccount.Role);

			var result = await _roleManager.CreateAsync(adminRole);
			// Check if Admin was not created
            if (!result.Succeeded)
            {
				var errorMessage = result.Errors.Select(e => e.Description);
				throw new Exception(string.Join(Environment.NewLine, errorMessage));
            }
        }

        // Seed Admin User

        // Seed Initial Categories
    }
}

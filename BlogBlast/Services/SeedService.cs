using BlogBlast.Data;
using Microsoft.AspNetCore.Identity;

namespace BlogBlast.Services;


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


		// Seed Admin User

		// Seed Initial Categories
	}
}

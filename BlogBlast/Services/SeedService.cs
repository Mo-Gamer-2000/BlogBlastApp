/*
 * This class defines the SeedService, which provides methods for seeding initial data into the database.
 */

using BlogBlast.Data;
using BlogBlast.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogBlast.Services
{
    // Represents the interface for seeding initial data
    public interface ISeedService
    {
        Task SeedDataAsync(); // Seeds initial data asynchronously
    }

    // Represents the service class for seeding initial data
    public class SeedService : ISeedService
    {
        private readonly ApplicationDbContext _context; // Database context
        private readonly IUserStore<ApplicationUser> _userStore; // User store
        private readonly UserManager<ApplicationUser> _userManager; // User manager
        private readonly RoleManager<IdentityRole> _roleManager; // Role manager
        private readonly IConfiguration _configuration; // Configuration

        public SeedService(ApplicationDbContext context, IUserStore<ApplicationUser> userStore, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _context = context; // Initializes the context
            _userStore = userStore; // Initializes the user store
            _userManager = userManager; // Initializes the user manager
            _roleManager = roleManager; // Initializes the role manager
            _configuration = configuration; // Initializes the configuration
        }

        // Automatically migrates the database - WARNING only for Development Mode - REMOVE THIS BEFORE PRODUCTION
        private async Task MigrateDatabaseAsync()
        {
#if DEBUG
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                await _context.Database.MigrateAsync();
            }
#endif
        }

        // Seeds initial data asynchronously
        public async Task SeedDataAsync()
        {
            // Access admin account constants from IConfiguration
            string? adminName = _configuration["AdminAccount:Name"];
            string? adminEmail = _configuration["AdminAccount:Email"];
            string? adminRole = _configuration["AdminAccount:Role"];
            string? adminPassword = _configuration["AdminAccount:Password"];

            await MigrateDatabaseAsync(); // Automatically migrate the database

            // Seed Admin Role
            // Check if the admin role exists in the database
            if (await _roleManager.FindByNameAsync(adminRole!) is null)
            {
                // If the admin role does not exist, create it
                var role = new IdentityRole(adminRole!);

                var result = await _roleManager.CreateAsync(role);
                // Check if the admin role creation was successful
                if (!result.Succeeded)
                {
                    var errorMessage = result.Errors.Select(e => e.Description);
                    throw new Exception($"An Error Occurred While Creating an Admin Role {Environment.NewLine}{string.Join(Environment.NewLine, errorMessage)}");
                }
            }

            // Seed Admin User
            var adminUser = await _userManager.FindByEmailAsync(adminEmail!);
            if (adminUser is null)
            {
                // If the admin user does not exist, create it
                adminUser = new ApplicationUser();
                adminUser.Name = adminName!;
                await _userStore.SetUserNameAsync(adminUser, adminEmail, CancellationToken.None);
                var emailStore = (IUserEmailStore<ApplicationUser>)_userStore;
                await emailStore.SetEmailAsync(adminUser, adminEmail, CancellationToken.None);
                var result = await _userManager.CreateAsync(adminUser, adminPassword!);

                // Check if the admin user creation was successful
                if (!result.Succeeded)
                {
                    var errorMessage = result.Errors.Select(e => e.Description);
                    throw new Exception($"An Error Occurred While Creating an Admin User {Environment.NewLine}{string.Join(Environment.NewLine, errorMessage)}");
                }
            }

            // Seed Initial Categories
            // Check if there are no categories in the database
            if (!await _context.Categories.AsNoTracking().AnyAsync())
            {
                // If there are no categories, create initial categories
                await _context.Categories.AddRangeAsync(Category.GetSeedCategories());
                await _context.SaveChangesAsync();
            }
        }
    }
}
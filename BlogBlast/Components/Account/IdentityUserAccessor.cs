/*
 * This class defines a user accessor for retrieving the current user in the BlogBlast application.
 */

using BlogBlast.Data;
using Microsoft.AspNetCore.Identity;

namespace BlogBlast.Components.Account
{
    internal sealed class IdentityUserAccessor
    {
        private readonly UserManager<ApplicationUser> userManager; // User manager for managing users
        private readonly IdentityRedirectManager redirectManager; // Redirect manager for managing redirects

        public IdentityUserAccessor(UserManager<ApplicationUser> userManager, IdentityRedirectManager redirectManager)
        {
            this.userManager = userManager; // Initialises user manager
            this.redirectManager = redirectManager; // Initialises redirect manager
        }

        // Gets the required user asynchronously
        public async Task<ApplicationUser> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                // Redirects with status message if user is null
                redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
            }

            return user;
        }
    }
}
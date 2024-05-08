/*
 * This class defines extension methods for mapping additional Identity endpoints in the BlogBlast application.
 * These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
 */

using BlogBlast.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Routing
{
    internal static class IdentityComponentsEndpointRouteBuilderExtensions
    {
        // Maps additional Identity endpoints required by the Identity Razor components
        public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
        {
            ArgumentNullException.ThrowIfNull(endpoints); // Ensures endpoints is not null

            // Maps endpoints under the /Account group
            var accountGroup = endpoints.MapGroup("/Account");

            // Maps the /Logout endpoint for signing out users
            accountGroup.MapPost("/Logout", async (
                ClaimsPrincipal user, // Represents the user
                SignInManager<ApplicationUser> signInManager, // Manages sign-in operations
                [FromForm] string returnUrl) => // Returns the URL to redirect after logout
            {
                await signInManager.SignOutAsync(); // Signs out the user
                return TypedResults.LocalRedirect($"~/{returnUrl}"); // Redirects to the specified return URL
            });

            return accountGroup; // Returns the configured endpoint group
        }
    }
}
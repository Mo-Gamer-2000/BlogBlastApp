/*
 * This class defines a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
 * every 30 minutes an interactive circuit is connected.
 */

using BlogBlast.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BlogBlast.Components.Account
{
    internal sealed class IdentityRevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
    {
        private readonly IServiceScopeFactory scopeFactory; // Service scope factory for creating new service scopes
        private readonly IOptions<IdentityOptions> options; // Identity options

        public IdentityRevalidatingAuthenticationStateProvider(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory scopeFactory,
            IOptions<IdentityOptions> options)
            : base(loggerFactory) // Initialises base class with logger factory
        {
            this.scopeFactory = scopeFactory; // Initialises service scope factory
            this.options = options; // Initialises Identity options
        }

        // Specifies the interval for revalidation
        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        // Validates the authentication state by revalidating the security stamp
        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            // Get the user manager from a new scope to ensure it fetches fresh data
            await using var scope = scopeFactory.CreateAsyncScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            return await ValidateSecurityStampAsync(userManager, authenticationState.User);
        }

        // Validates the security stamp
        private async Task<bool> ValidateSecurityStampAsync(UserManager<ApplicationUser> userManager, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user is null)
            {
                return false;
            }
            else if (!userManager.SupportsUserSecurityStamp)
            {
                return true;
            }
            else
            {
                var principalStamp = principal.FindFirstValue(options.Value.ClaimsIdentity.SecurityStampClaimType);
                var userStamp = await userManager.GetSecurityStampAsync(user);
                return principalStamp == userStamp;
            }
        }
    }
}
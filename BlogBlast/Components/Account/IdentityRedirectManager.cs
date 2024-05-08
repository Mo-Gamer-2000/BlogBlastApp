/*
 * This class defines a manager for redirecting within Identity components in the BlogBlast application.
 */

using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace BlogBlast.Components.Account
{
    internal sealed class IdentityRedirectManager
    {
        private readonly NavigationManager navigationManager; // NavigationManager for navigation purposes

        public const string StatusCookieName = "Identity.StatusMessage"; // Name of the status message cookie

        // Cookie builder for status message
        private static readonly CookieBuilder StatusCookieBuilder = new()
        {
            SameSite = SameSiteMode.Strict,
            HttpOnly = true,
            IsEssential = true,
            MaxAge = TimeSpan.FromSeconds(5),
        };

        public IdentityRedirectManager(NavigationManager navigationManager)
        {
            this.navigationManager = navigationManager; // Initialises NavigationManager
        }

        // Redirects to the specified URI
        [DoesNotReturn]
        public void RedirectTo(string? uri)
        {
            uri ??= "";

            // Prevents open redirects
            if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
            {
                uri = navigationManager.ToBaseRelativePath(uri);
            }

            // During static rendering, NavigateTo throws a NavigationException which is handled by the framework as a redirect.
            // So as long as this is called from a statically rendered Identity component, the InvalidOperationException is never thrown.
            navigationManager.NavigateTo(uri);
            throw new InvalidOperationException($"{nameof(IdentityRedirectManager)} can only be used during static rendering.");
        }

        // Redirects to the specified URI with query parameters
        [DoesNotReturn]
        public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
        {
            var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
            var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
            RedirectTo(newUri);
        }

        // Redirects to the specified URI with a status message
        [DoesNotReturn]
        public void RedirectToWithStatus(string uri, string message, HttpContext context)
        {
            context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));
            RedirectTo(uri);
        }

        // Gets the current page path
        private string CurrentPath => navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);

        // Redirects to the current page
        [DoesNotReturn]
        public void RedirectToCurrentPage() => RedirectTo(CurrentPath);

        // Redirects to the current page with a status message
        [DoesNotReturn]
        public void RedirectToCurrentPageWithStatus(string message, HttpContext context)
            => RedirectToWithStatus(CurrentPath, message, context);
    }
}
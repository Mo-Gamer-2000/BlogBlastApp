using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BlogBlast
{
    // Static partial class to hold extension methods
    public static partial class Extensions
    {
        // Extension method to get the user name from ClaimsPrincipal
        public static string GetUserName(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(AppConstants.ClaimNames.FullName)!;

        // Extension method to get the user ID from ClaimsPrincipal
        public static string GetUserId(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Extension method to convert text to a URL-friendly slug
        public static string ToSlug(this string text)
        {
            // Convert the text to lower case and replace special characters with "-"
            text = SlugRegex().Replace(text.ToLowerInvariant(), "-");

            // Replace consecutive dashes with single dash and trim leading/trailing dashes
            return text.Replace("--", "-").Trim('-');
        }

        // Extension method to format DateTime? for display
        public static string ToDisplay(this DateTime? dateTime) => dateTime?.ToString("MMM dd") ?? string.Empty;

        // Custom regular expression generator method for slug conversion
        [GeneratedRegex(@"[^0-9a-z_]")] // Regex pattern to remove characters not matching alphanumeric or underscore
        private static partial Regex SlugRegex();
    }
}

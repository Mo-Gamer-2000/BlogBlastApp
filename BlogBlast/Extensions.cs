using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BlogBlast
{
    // Static partial class to hold extension methods
    public static partial class Extensions
    {
        // Extension method to get the user name from ClaimsPrincipal
        // Retrieves the full name of the user from the ClaimsPrincipal object.
        public static string GetUserName(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(AppConstants.ClaimNames.FullName)!;

        // Extension method to get the user ID from ClaimsPrincipal
        // Retrieves the unique identifier of the user from the ClaimsPrincipal object.
        public static string GetUserId(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Extension method to convert text to a URL-friendly slug
        // Converts the provided text into a URL-friendly slug format.
        public static string ToSlug(this string text)
        {
            // Convert the text to lower case and replace special characters with "-"
            text = SlugRegex().Replace(text.ToLowerInvariant(), "-");

            // Replace consecutive dashes with single dash and trim leading/trailing dashes
            return text.Replace("--", "-").Trim('-');
        }

        // Extension method to format DateTime? for display
        // Formats the nullable DateTime object into a string for display purposes.
        public static string ToDisplay(this DateTime? dateTime) => dateTime?.ToString("MMM dd") ?? string.Empty;

        // Custom regular expression generator method for slug conversion
        // Generates a regular expression pattern to remove characters not matching alphanumeric or underscore.
        [GeneratedRegex(@"[^0-9a-z_]")] // Regex pattern to remove characters not matching alphanumeric or underscore
        private static partial Regex SlugRegex();
    }
}
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BlogBlast;
public static partial class Extensions
{
    public static string GetUserName(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(AppConstants.ClaimNames.FullName)!;

    public static string GetUserId(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public static string ToSlug(this string text) 
    {
        // Convert the text to lower case
        text = SlugRegex().Replace(text.ToLowerInvariant(), "-"); // Here, we replace them with "-"
        return text.Replace("--", "-").Trim('-'); 
        // ^^^ And If there is "--" then replace them with "-"
        // and then trim "-" from start and the end of the slug
    }

    [GeneratedRegex(@"[^0-9a-z_]")] // Not allowed characters then ^^^
    private static partial Regex SlugRegex();
}

namespace BlogBlast;

// Utility class to provide common functions
public static class Utility
{
    // Method to get the formatted page title
    // Concatenates the provided title with the application name "BlogBlast" separated by a pipe character
    public static string GetPageTitle(string title) => $"{title} | BlogBlast";
}
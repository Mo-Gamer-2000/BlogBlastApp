/*
 * This class defines constant values used throughout the application.
 * It provides a convenient way to organize and access constants related to authentication and authorization.
 */
namespace BlogBlast
{
    public static class AppConstants
    {
        /*
         * This nested class holds constants related to claim names used in authentication and authorization.
         * It provides a clean way to group these constants together for easy reference.
         */
        public static class ClaimNames
        {
            // Constant string representing the name of the full name claim.
            public const string FullName = "FullName";
        }
    }
}
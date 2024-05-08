/*
 * This class defines the ApplicationUser class, which represents the application users in the BlogBlast application.
 */

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlogBlast.Data
{
    // Represents an application user
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(25)]
        public string Name { get; set; } // Name of the user
    }
}
/*
 * This class defines the Subscription entity in the BlogBlast application.
 */

using System.ComponentModel.DataAnnotations;

namespace BlogBlast.Data.Entities
{
    public class Subscription
    {
        public long Id { get; set; } // Unique identifier for the subscription

        [EmailAddress, Required, MaxLength(150)]
        public string Email { get; set; } // Email address of the subscriber

        [Required, MaxLength(25)]
        public string Name { get; set; } // Name of the subscriber

        public DateTime SubscribedOn { get; set; } // Date and time when the subscription was made
    }
}
/*
 * This class defines the Post entity in the BlogBlast application.
 */

using System.ComponentModel.DataAnnotations;

namespace BlogBlast.Data.Entities
{
    public class Post
    {
        public int Id { get; set; } // Unique identifier for the post

        [Required, MaxLength(100)]
        public string Title { get; set; } // Title of the post

        [MaxLength(125)]
        public string Slug { get; set; } // Slug of the post

        [MaxLength(100)]
        public string Image { get; set; } // Image URL of the post

        [Required, MaxLength(500)]
        public string Introduction { get; set; } // Introduction of the post

        public string Content { get; set; } // Content of the post

        [Range(1, short.MaxValue, ErrorMessage = "The Category field is required.")]
        public short CategoryId { get; set; } // ID of the category to which the post belongs

        public string UserId { get; set; } // ID of the user who created the post

        public int ViewCount { get; set; } // Number of views for the post

        public bool IsPublished { get; set; } // Indicates if the post is published

        public bool IsFeatured { get; set; } // Indicates if the post is featured

        public DateTime CreatedAt { get; set; } // Date and time when the post was created

        public DateTime? PublishedAt { get; set; } // Date and time when the post was published

        public virtual Category Category { get; set; } // Navigation property for the category of the post

        public virtual ApplicationUser User { get; set; } // Navigation property for the user who created the post
    }
}
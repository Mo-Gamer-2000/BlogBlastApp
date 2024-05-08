/*
 * This class defines the Category entity in the BlogBlast application.
 */

using System.ComponentModel.DataAnnotations;

namespace BlogBlast.Data.Entities
{
    // Represents a category
    public class Category
    {
        // Attributes to store category data
        public short Id { get; set; } // Unique identifier for the category

        [Required, MaxLength(50)]
        public string Name { get; set; } // Name of the category

        [MaxLength(75)]
        public string Slug { get; set; } // Slug of the category

        public bool VisibleOnNavbar { get; set; } // Indicates if the category is visible on the navigation bar

        // Creates a deep copy of the category object
        public Category Clone() => (MemberwiseClone() as Category)!;

        // Generates an array of seed categories
        public static Category[] GetSeedCategories()
        {
            return new[]
            {
                // Seed categories with predefined names, slugs, and navbar visibility.
                new Category { Name = "Technology", Slug = "technology", VisibleOnNavbar = true },
                new Category { Name = "Travel", Slug = "travel", VisibleOnNavbar = true },
                new Category { Name = "Food", Slug = "food", VisibleOnNavbar = true },
                new Category { Name = "Fitness", Slug = "fitness", VisibleOnNavbar = true },
                new Category { Name = "Fashion", Slug = "fashion", VisibleOnNavbar = true },
                new Category { Name = "Health", Slug = "health", VisibleOnNavbar = false },
                new Category { Name = "Finance", Slug = "finance", VisibleOnNavbar = false },
                new Category { Name = "Entertainment", Slug = "entertainment", VisibleOnNavbar = true },
                new Category { Name = "Lifestyle", Slug = "lifestyle", VisibleOnNavbar = false },
                new Category { Name = "Education", Slug = "education", VisibleOnNavbar = false }
            };
        }
    }
}
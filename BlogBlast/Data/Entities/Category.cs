using System.ComponentModel.DataAnnotations;

namespace BlogBlast.Data.Entities;
// Defined Category class
public class Category
{
    // Attributes to store category data
    public short Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; }
    [Required, MaxLength(75)]
    public string Slug { get; set; }
    public bool VisibleOnNavbar { get; set; }

    // Method to create a deep copy of the category object, this for HandleEditCategory method to be used
    public Category Clone() => (MemberwiseClone() as Category)!;

    // Method to generate an array of seed categories
    public static Category[] GetSeedCategories()
    {
        return
            [
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
			];
    }
}

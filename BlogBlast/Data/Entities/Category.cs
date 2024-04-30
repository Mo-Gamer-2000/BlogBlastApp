using System.ComponentModel.DataAnnotations;

namespace BlogBlast.Data.Entities;
public class Category
{
    public short Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; }
    [Required, MaxLength(75)]
    public string Slug { get; set; }
    public bool VisibleOnNavbar { get; set; }

	// Created an Array for Seed Categories
    public static Category[] GetSeedCategories()
    {
        return
            [
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

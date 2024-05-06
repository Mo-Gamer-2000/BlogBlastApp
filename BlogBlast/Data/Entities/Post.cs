using System.ComponentModel.DataAnnotations;

namespace BlogBlast.Data.Entities;

public class Post
{
	public int Id { get; set; }

	[Required, MaxLength(100)]
	public string Title { get; set; }

    [MaxLength(125)]
    public string Slug { get; set; }

	[MaxLength(100)]
	public string Image { get; set; }

	[Required, MaxLength(500)]
	public string Introduction { get; set; }

	public string Content { get; set; }

	[Range(1, short.MaxValue, ErrorMessage = "The Category field is required.")] // Setting CategoryID to start prom 1 to prevent having a category with ID 0, which will be in null in my case
    public short CategoryId { get; set; }

    public string UserId { get; set; }

	public int ViewCount { get; set; }

	public bool IsPublished { get; set; }

    public bool IsFeatured { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? PublishedAt { get; set; }

	public virtual Category Category { get; set;}

    public virtual ApplicationUser User { get; set; }
}

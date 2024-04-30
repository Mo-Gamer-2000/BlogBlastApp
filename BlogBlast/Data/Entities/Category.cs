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
}

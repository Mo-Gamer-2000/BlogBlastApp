using System.ComponentModel.DataAnnotations;

namespace BlogBlast.Data.Entities;
public class Category
{
    public short Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; }
    public string Slug { get; set; }
    public bool VisibleOnNavbar { get; set; }
}

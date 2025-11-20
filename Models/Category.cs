using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartInventorySystem.Models;

[Index("Name", Name = "UQ_Categories_Name", IsUnique = true)]
public partial class Category
{
    [Key]
    public int CategoryId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("ImageURL")]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<AutoPart> AutoParts { get; set; } = new List<AutoPart>();
}

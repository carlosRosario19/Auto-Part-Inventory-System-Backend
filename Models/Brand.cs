using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoPartInventorySystem.Models;

[Index("Name", Name = "UQ_Brands_Name", IsUnique = true)]
public partial class Brand
{
    [Key]
    public int BrandId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("ImageURL")]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [InverseProperty("Brand")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    [ForeignKey("BrandId")]
    [InverseProperty("Brands")]
    public virtual ICollection<AutoPart> AutoParts { get; set; } = new List<AutoPart>();
}

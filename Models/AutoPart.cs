using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoPartInventorySystem.Models;

public partial class AutoPart
{
    [Key]
    public int AutoPartId { get; set; }

    [StringLength(200)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Column("ImageURL")]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Cost { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("AutoParts")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("AutoPartId")]
    [InverseProperty("AutoParts")]
    public virtual ICollection<Brand> Brands { get; set; } = new List<Brand>();

    [ForeignKey("AutoPartId")]
    [InverseProperty("AutoParts")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}

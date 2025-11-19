using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AutoPartInventorySystem.Models;

public partial class Vehicle
{
    [Key]
    public int VehicleId { get; set; }

    public int BrandId { get; set; }

    [StringLength(100)]
    public string Model { get; set; } = null!;

    public int StartYear { get; set; }

    public int? EndYear { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Vehicles")]
    public virtual Brand Brand { get; set; } = null!;

    [ForeignKey("VehicleId")]
    [InverseProperty("Vehicles")]
    public virtual ICollection<AutoPart> AutoParts { get; set; } = new List<AutoPart>();
}

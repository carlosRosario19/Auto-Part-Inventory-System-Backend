using System.ComponentModel.DataAnnotations;

namespace AutoPartInventorySystem.DTOs.AutoPart
{
    public class LinkPartWithVehicleDto
    {
        [Required]
        public int AutoPartId {  get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int StartYear { get; set; }
        [Required]
        public int EndYear { get; set; }
    }
}

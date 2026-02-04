using System.ComponentModel.DataAnnotations;

namespace Tawsella.Domain.DTOs.OrderDTOs
{
    public class CalculatePriceDto
    {
        [Required]
        public decimal PickupLatitude { get; set; }
        
        [Required]
        public decimal PickupLongitude { get; set; }
        
        [Required]
        public decimal DropoffLatitude { get; set; }
        
        [Required]
        public decimal DropoffLongitude { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string PackageSize { get; set; }  // Small, Medium, Large
        
        public decimal? PackageWeight { get; set; }
    }
}
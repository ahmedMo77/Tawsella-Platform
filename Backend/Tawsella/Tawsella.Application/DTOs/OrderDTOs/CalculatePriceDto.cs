using System.ComponentModel.DataAnnotations;

namespace Tawsella.Application.DTOs.OrderDTOs
{
    public class CalculatePriceDto
    {
        public decimal PickupLatitude { get; set; }
        public decimal PickupLongitude { get; set; }
        public decimal DropoffLatitude { get; set; }
        public decimal DropoffLongitude { get; set; }
        public string PackageSize { get; set; }  // Small, Medium, Large
        public decimal? PackageWeight { get; set; }
        public bool IsFragile { get; set; }
    }
}
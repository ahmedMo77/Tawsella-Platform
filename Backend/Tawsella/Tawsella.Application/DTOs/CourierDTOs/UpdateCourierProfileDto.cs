using System.ComponentModel.DataAnnotations;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.DTOs.CourierDTOs
{
    public class UpdateCourierProfileDto
    {
        [Required, MaxLength(200)]
        public string FullName { get; set; }

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; }

        public VehicleType? VehicleType { get; set; }

        [Required, MaxLength(20)]
        public string VehiclePlateNumber { get; set; }
    }
}

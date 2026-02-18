using MediatR;
using System.ComponentModel.DataAnnotations;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Authentication.Commands.RegisterCourier
{
    public class RegisterCourierCommand : IRequest<RegisterCourierCommandResponse>
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        public string NationalId { get; set; }

        [Required]
        public VehicleType VehicleType { get; set; }

        [Required]
        public string VehiclePlateNumber { get; set; }

        [Required]
        public string LicenseNumber { get; set; }

        [Required]
        public DateTime LicenseExpiryDate { get; set; }
    }
}

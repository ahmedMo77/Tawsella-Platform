using MediatR;
using System.ComponentModel.DataAnnotations;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Couriers.Commands.UpdateCourierProfile
{
    public class UpdateCourierProfileCommand : IRequest<UpdateCourierProfileCommandResponse>
    {
        [MaxLength(100)]
        public string FullName { get; set; }

        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public string? VehicleType { get; set; }

        [MaxLength(20)]
        public string VehiclePlateNumber { get; set; }
    }
}

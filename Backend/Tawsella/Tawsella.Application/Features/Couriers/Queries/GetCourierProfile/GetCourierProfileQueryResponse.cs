using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Couriers.Queries.GetCourierProfile
{
    public class GetCourierProfileQueryResponse
    {
        public CourierProfileDto Profile { get; set; }

        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public VehicleType VehicleType { get; set; }
        public string VehiclePlateNumber { get; set; }
        public bool IsApproved { get; set; }
        public bool IsOnline { get; set; }
        public decimal WalletBalance { get; set; }
    }
}

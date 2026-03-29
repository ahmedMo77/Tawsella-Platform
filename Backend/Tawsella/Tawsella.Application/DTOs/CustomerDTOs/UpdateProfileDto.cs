using System.ComponentModel.DataAnnotations;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.DTOs.CustomerDTOs
{
    public class UpdateProfileDto
    {
        [MaxLength(500)]
        public string DefaultPickupAddress { get; set; }

        public decimal? DefaultPickupLatitude { get; set; }

        public decimal? DefaultPickupLongitude { get; set; }
        [MaxLength(50)]
        public string DefaultPickupLabel { get; set; }  // Home, Work, Gym, etc.
        public PaymentMethod? PreferredPaymentMethod { get; set; }
    }
}
using MediatR;
using System.ComponentModel.DataAnnotations;
using Tawsella.Application.Enums;

namespace Tawsella.Application.Features.Customers.Commands.UpdateCustomerProfile
{
    public class UpdateCustomerProfileCommand : IRequest<UpdateCustomerProfileCommandResponse>
    {
        [MaxLength(500)]
        public string DefaultPickupAddress { get; set; }

        public decimal? DefaultPickupLatitude { get; set; }

        public decimal? DefaultPickupLongitude { get; set; }

        [MaxLength(50)]
        public string DefaultPickupLabel { get; set; }

        public PaymentMethod? PreferredPaymentMethod { get; set; }
    }
}

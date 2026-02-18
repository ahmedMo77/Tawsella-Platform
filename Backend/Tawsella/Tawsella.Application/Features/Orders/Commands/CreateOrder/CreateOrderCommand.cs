using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand:IRequest<CreateOrderCommandResponse>
    {
        [Required]
        [MaxLength(500)]
        public string PickupAddress { get; set; }

        [Required]
        public decimal PickupLatitude { get; set; }

        [Required]
        public decimal PickupLongitude { get; set; }

        [Required]
        [MaxLength(100)]
        public string PickupContactName { get; set; }

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PickupContactPhone { get; set; }

        // Dropoff Details
        [Required]
        [MaxLength(500)]
        public string DropoffAddress { get; set; }

        [Required]
        public decimal DropoffLatitude { get; set; }

        [Required]
        public decimal DropoffLongitude { get; set; }

        [Required]
        [MaxLength(100)]
        public string DropoffContactName { get; set; }

        [Required]
        [Phone]
        [MaxLength(20)]
        public string DropoffContactPhone { get; set; }

        // Package Details
        [Required]
        [MaxLength(50)]
        public string PackageSize { get; set; }  // Small, Medium, Large

        public decimal? PackageWeight { get; set; }

        [MaxLength(500)]
        public string PackageNotes { get; set; }

        // Payment
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}

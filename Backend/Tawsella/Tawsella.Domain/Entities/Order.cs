using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        // Courier
        public string? CourierId { get; set; } // The assigned courier, nullable until assigned
        public Courier Courier { get; set; }

        // Pickup Details
        public string PickupAddress { get; set; }
        public decimal PickupLatitude { get; set; }
        public decimal PickupLongitude { get; set; }
        public string PickupContactName { get; set; }
        public string PickupContactPhone { get; set; }

        // Dropoff Details
        public string DropoffAddress { get; set; }
        public decimal DropoffLatitude { get; set; }
        public decimal DropoffLongitude { get; set; }
        public string DropoffContactName { get; set; }
        public string DropoffContactPhone { get; set; }

        // Package Details
        public string PackageSize { get; set; }
        public decimal? PackageWeight { get; set; }
        public string PackageNotes { get; set; }
        public decimal EstimatedPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? CourierEarnings { get; set; }
        public decimal? PlatformCommission { get; set; }

        // Status & Timing
        public OrderStatus Status { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string CancellationReason { get; set; }


        // Payment => maybe integrate with a Payment entity in future for more details
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime? PaidAt { get; set; }

        // Navigation
        public Review Review { get; set; }
        public ICollection<OrderStatusHistory> StatusHistory { get; set; }
    }
}
    

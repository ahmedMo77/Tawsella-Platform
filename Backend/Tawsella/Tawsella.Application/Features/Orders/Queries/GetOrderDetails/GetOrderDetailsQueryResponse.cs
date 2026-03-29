using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Enums;

namespace Tawsella.Application.Features.Orders.Queries.GetOrderDetails
{
    public class GetOrderDetailsQueryResponse
    {
        public string Id { get; set; }
        public string OrderNumber { get; set; }

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

        // Pricing
        public decimal EstimatedPrice { get; set; }
        public decimal? FinalPrice { get; set; }

        // Status & Timing
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string CancellationReason { get; set; }

        // Payment
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        // Courier Info (if assigned)
        public string CourierId { get; set; }
        public string CourierName { get; set; }
        public string CourierPhone { get; set; }
    }
}

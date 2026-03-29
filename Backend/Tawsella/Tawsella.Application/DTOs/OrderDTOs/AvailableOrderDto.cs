using System;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.DTOs.OrderDTOs
{
    /// <summary>
    /// Order available for couriers to apply (Pending, no courier assigned yet).
    /// </summary>
    public class AvailableOrderDto
    {
        public string Id { get; set; }
        public string OrderNumber { get; set; }
        public string PickupAddress { get; set; }
        public decimal PickupLatitude { get; set; }
        public decimal PickupLongitude { get; set; }
        public string DropoffAddress { get; set; }
        public decimal DropoffLatitude { get; set; }
        public decimal DropoffLongitude { get; set; }
        public string PackageSize { get; set; }
        public decimal? PackageWeight { get; set; }
        public decimal EstimatedPrice { get; set; }
        public decimal? CourierEarnings { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasApplied { get; set; }
    }
}

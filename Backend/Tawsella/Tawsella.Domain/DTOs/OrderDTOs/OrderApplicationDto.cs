using System;
using Tawsella.Domain.Enums;

namespace Tawsella.Domain.DTOs.OrderDTOs
{
    public class OrderApplicationDto
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public OrderApplicationStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string RejectedReason { get; set; }
        public string PickupAddress { get; set; }
        public string DropoffAddress { get; set; }
        public decimal? CourierEarnings { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}

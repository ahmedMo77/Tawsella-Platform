using System;
using Tawsella.Application.Enums;

namespace Tawsella.Application.Entities
{
    public class OrderApplication : BaseEntity
    {
        public string OrderId { get; set; }
        public Order Order { get; set; }

        public string CourierId { get; set; }
        public Courier Courier { get; set; }

        public OrderApplicationStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? RejectedReason { get; set; }
    }
}

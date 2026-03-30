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

        // 1. Parties (customer and captain)
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string? CourierId { get; set; }
        public Courier Courier { get; set; }

        // 2. /Locations (pickup and dropoff)
        public OrderContact Pickup { get; set; } = new();
        public OrderContact Dropoff { get; set; } = new();

        // 3. Package details and finances (تخص الأوردر نفسه)
        public OrderPackage Package { get; set; } = new();
        public OrderFinances Money { get; set; } = new();

        // 4. Status and timestamps
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime? AcceptedAt { get; set; }
        public DateTime? PickedUpAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancellationReason { get; set; }

        // 5. Payment details
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime? PaidAt { get; set; }

        // 6. References to related entities 
        public Review Review { get; set; }
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
    }
}
    

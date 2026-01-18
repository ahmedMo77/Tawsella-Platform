using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Domain.Entities
{
    public class OrderStatusHistory
    {
        public string Id { get; set; }

        public string OrderId { get; set; }
        public Order Order { get; set; }

        public OrderStatus Status { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }
    }
}

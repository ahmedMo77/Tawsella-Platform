using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Domain.Entities
{
    public class OrderStatusHistory:BaseEntity
    {

        public string OrderId { get; set; }
        public Order Order { get; set; }

        public OrderStatus Status { get; set; }
        public string Notes { get; set; }
        public string? CreatedBy { get; set; }
    }
}

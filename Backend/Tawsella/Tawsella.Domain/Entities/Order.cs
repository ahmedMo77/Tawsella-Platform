using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.Entities
{
    public class Order
    {
        public string Id { get; set; }

        public string Description { get; set; }
        public string PickupAddress { get; set; }
        public string DeliveryAddress { get; set; }

        public decimal Weight { get; set; }
        public decimal Price { get; set; }

        public string CreatedByUserId { get; set; }
        public AppUser CreatedByUser { get; set; }

        public string? CourierId { get; set; }
        public Courier? Courier { get; set; }
    }

}

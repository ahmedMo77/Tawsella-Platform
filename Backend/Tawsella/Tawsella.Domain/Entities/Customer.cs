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
    public class Customer : BaseEntity
    {
        public AppUser User { get; set; }

        public Location DefaultPickupLocation { get; set; } = new();
        public string DefaultPickupLabel { get; set; }

        public int CompletedOrders { get; set; }
        public PaymentMethod PreferredPaymentMethod { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

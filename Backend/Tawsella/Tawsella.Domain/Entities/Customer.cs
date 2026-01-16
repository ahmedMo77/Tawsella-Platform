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
    public class Customer 
    {
        public string Id { get; set; }
        public AppUser User { get; set; }

        [MaxLength(200)]
        public string DefaultPickupAddress { get; set; }

        [Column(TypeName = "decimal(10,8)")]
        public decimal? DefaultPickupLatitude { get; set; }

        [Column(TypeName = "decimal(11,8)")]
        public decimal? DefaultPickupLongitude { get; set; }

        [MaxLength(200)]
        public string DefaultDropoffAddress { get; set; }

        [Column(TypeName = "decimal(10,8)")]
        public decimal? DefaultDropoffLatitude { get; set; }

        [Column(TypeName = "decimal(11,8)")]
        public decimal? DefaultDropoffLongitude { get; set; }
        public PaymentMethod PreferredPaymentMethod { get; set; }
        public int TotalOrdersCount { get; set; }

        public int CompletedOrdersCount { get; set; }

        public int CancelledOrdersCount { get; set; }

        public ICollection<Order> Orders { get; set; } 
        public ICollection<Review> Reviews { get; set; }
    }
}

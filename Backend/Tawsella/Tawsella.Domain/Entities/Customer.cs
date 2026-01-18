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
        public string DefaultPickupAddress { get; set; }
        public decimal? DefaultPickupLatitude { get; set; }
        public decimal? DefaultPickupLongitude { get; set; }
        public string DefaultPickupLabel { get; set; }  // Home, Work, Gym

        public PaymentMethod PreferredPaymentMethod { get; set; }

        // Dropoff details maybe not relevant for a customer profile, as dropoff locations vary per order.

        //public string DefaultDropoffAddress { get; set; }
        //public decimal? DefaultDropoffLatitude { get; set; }
        //public decimal? DefaultDropoffLongitude { get; set; }

        public AppUser User { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}

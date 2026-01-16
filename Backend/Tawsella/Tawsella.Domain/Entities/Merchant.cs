using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Domain.Entities
{
    public class Merchant 
    {

        [Required]
        [MaxLength(200)]
        public string BusinessName { get; set; }

        [MaxLength(100)]
        public string BusinessRegistrationNumber { get; set; }

        [MaxLength(500)]
        public string BusinessAddress { get; set; }

        [MaxLength(500)]
        public string BusinessCategory { get; set; }

        public bool IsApproved { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public Guid? ApprovedBy { get; set; }

        public Guid? SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }

        public string MerchantId { get; set; }
        public AppUser User { get; set; }

        // Navigation
        public ICollection<Order> Orders { get; set; }
    }
}

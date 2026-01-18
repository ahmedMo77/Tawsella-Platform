using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Domain.Entities
{
    public class Merchant : BaseEntity
    {
        // Business Details
        public string BusinessName { get; set; }
        public string BusinessRegistrationNumber { get; set; }
        public string BusinessAddress { get; set; }
        public string BusinessCategory { get; set; }

        // Approval Status
        public bool IsApproved { get; set; }
        public DateTime ApprovedAt { get; set; }
        public string ApprovedBy { get; set; }

        public string? SubscriptionId { get; set; }

        // Navigation
        public Subscription Subscription { get; set; }
        public AppUser User { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}

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
    public class SubscriptionPlan
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SubscriptionType Type { get; set; }
        public BillingPeriod BillingPeriod { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string Features { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}

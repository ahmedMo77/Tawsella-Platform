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
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public SubscriptionType Type { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public BillingPeriod BillingPeriod { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(1000)]
        public string Features { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}

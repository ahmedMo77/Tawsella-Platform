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
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; }

        // Customer or Merchant
        public Guid? CustomerId { get; set; }
        public Customer Customer { get; set; }

        public Guid? MerchantId { get; set; }
        public Merchant Merchant { get; set; }

        // Courier or Company
        public Guid? CourierId { get; set; }
        public Courier Courier { get; set; }

        // Pickup Details
        [Required]
        [MaxLength(500)]
        public string PickupAddress { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,8)")]
        public decimal PickupLatitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(11,8)")]
        public decimal PickupLongitude { get; set; }

        [MaxLength(200)]
        public string PickupContactName { get; set; }

        [Phone]
        public string PickupContactPhone { get; set; }

        // Dropoff Details
        [Required]
        [MaxLength(500)]
        public string DropoffAddress { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,8)")]
        public decimal DropoffLatitude { get; set; }

        [Required]
        [Column(TypeName = "decimal(11,8)")]
        public decimal DropoffLongitude { get; set; }

        [MaxLength(200)]
        public string DropoffContactName { get; set; }

        [Phone]
        public string DropoffContactPhone { get; set; }

        // Package Details
        [MaxLength(100)]
        public string PackageSize { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal? PackageWeight { get; set; }

        [MaxLength(1000)]
        public string PackageNotes { get; set; }

        // Pricing
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal EstimatedPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? FinalPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? CourierEarnings { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PlatformCommission { get; set; }

        // Status & Timing
        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? AcceptedAt { get; set; }

        public DateTime? PickedUpAt { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        [MaxLength(500)]
        public string CancellationReason { get; set; }

        // Payment
        public PaymentMethod PaymentMethod { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public DateTime? PaidAt { get; set; }

        // Navigation
        public ICollection<OrderStatusHistory> StatusHistory { get; set; }
        public Review Review { get; set; }

    }
}
    

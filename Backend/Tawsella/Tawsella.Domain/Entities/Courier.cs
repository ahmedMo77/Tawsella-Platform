using MimeKit.Encodings;
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
    // Single Responsibility Principle not applied here for simplicity, maybe split into multiple related entities in future.
    // as vehicele details, location tracking could be separate entities.
    public class Courier : BaseEntity
    {
        public string NationalId { get; set; }

        // Vehicle Details
        public VehicleType VehicleType { get; set; } 
        public string VehiclePlateNumber { get; set; }
        public string? LicenseNumber { get; set; }  // if courier has a bycicle, he may not have a license
        public DateTime? LicenseExpiryDate { get; set; }

        // Approval & Status
        public bool IsApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public bool IsOnline { get; set; }
        public bool IsAvailable { get; set; }

        // Location Tracking to mapping and order assignment
        public decimal? CurrentLatitude { get; set; }
        public decimal? CurrentLongitude { get; set; }
        public DateTime? LastLocationUpdate { get; set; }

        // Relationships
        public string? WalletId { get; set; }
        public string? SubscriptionId { get; set; }

        public AppUser User { get; set; }
        public Wallet Wallet { get; set; }
        public Subscription Subscription { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
    }
}

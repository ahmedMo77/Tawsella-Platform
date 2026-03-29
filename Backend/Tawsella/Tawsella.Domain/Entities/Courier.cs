using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.Entities
{
    public class Courier : BaseEntity
    {
        public string NationalId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        // Vehicle Details
        public string VehicleType { get; set; }  // maybe enum in the future (Bike, Car, Van, etc.)
        public string VehiclePlateNumber { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }

        // Approval & Status
        public bool IsApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public bool IsOnline { get; set; }
        public bool IsAvailable { get; set; }

        // Location Tracking
        public decimal? CurrentLatitude { get; set; }
        public decimal? CurrentLongitude { get; set; }
        public DateTime? LastLocationUpdate { get; set; }

        // Relationships
        public AppUser User { get; set; }

        public string? WalletId { get; set; }
        public Wallet Wallet { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
    }
}

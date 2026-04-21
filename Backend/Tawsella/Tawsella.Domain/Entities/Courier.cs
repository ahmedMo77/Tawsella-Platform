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
        public AppUser User { get; set; }

        // 2. Personal & Professional Info
        public string NationalId { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        // 3. Vehicle Information
        public string? VehicleId { get; set; }
        public VehicleInfo Vehicle { get; set; }

        // 4. Status & Approval
        public bool IsApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public bool IsOnline { get; set; }
        public bool IsAvailable { get; set; }

        // 5. Current Location
        public Location CurrentLocation { get; set; } = new();
        public DateTime? LastLocationUpdate { get; set; }

        // 6. Financials & Records
        public string? WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public ICollection<Order> Orders { get; set; } 
        public ICollection<Review> ReviewsReceived { get; set; }
    }
}

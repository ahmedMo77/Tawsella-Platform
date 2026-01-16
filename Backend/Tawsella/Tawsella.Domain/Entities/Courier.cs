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
    // عامل التوصيل 
    public class Courier
    {
        public string Id { get; set; }
        public string NationalId { get; set; }
        public string VehicleType { get; set; }
        public string VehiclePlateNumber { get; set; }

        public string LicenseNumber { get; set; }

        public bool IsApproved { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public bool IsOnline { get; set; }

        public bool IsAvailable { get; set; }

        public decimal? CurrentLatitude { get; set; }

        public decimal? CurrentLongitude { get; set; }

        public DateTime? LastLocationUpdate { get; set; }
        public string CourierId { get; set; }
        public AppUser User { get; set; }
        public Guid? WalletId { get; set; }
        public Wallet Wallet { get; set; }

        public Guid? SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> ReviewsReceived { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Domain.Entities
{
    // عامل التوصيل 
    public class Courier
    {
        public string Id {  get; set; }
        public string NationalId { get; set; }
        public bool IsApproved { get; set; }
        public VehicleType VehicleType { get; set; }
        public string? LicensePlate { get; set; }

        public AppUser User { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
}

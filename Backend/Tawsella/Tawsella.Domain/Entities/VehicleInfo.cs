using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Domain.Entities
{
    [Owned]
    public class VehicleInfo
    {
        public VehicleType Type { get; set; }
        public string PlateNumber { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
    }
}

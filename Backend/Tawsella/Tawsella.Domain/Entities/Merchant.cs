using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Domain.Entities
{
    public class Merchant
    {
        public string Id { get; set; }
        public bool IsApproved { get; set; }
        public string BusinessName { get; set; }
        public string? BusinessAddress { get; set; }
        public BusinessCategory BusinessCategory { get; set; }

        public AppUser User { get; set; }
        public List<Order> Orders { get; set; } = new();
    }
}

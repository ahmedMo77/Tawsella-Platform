using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.Entities
{
    [Owned]
    public class OrderFinances
    {
        public decimal EstimatedPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? CourierEarnings { get; set; }
        public decimal? PlatformCommission { get; set; }
    }
}

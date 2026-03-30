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
    public class OrderPackage
    {
        public PackageSize Size { get; set; }
        public decimal? Weight { get; set; }
        public string? Notes { get; set; }
    }
}

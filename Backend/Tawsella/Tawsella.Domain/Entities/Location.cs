using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.Entities
{
    [Owned]
    public class Location
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? AddressName { get; set; }

        public Location() { }

        public Location(decimal lat, decimal lng, string? address = null)
        {
            Latitude = lat;
            Longitude = lng;
            AddressName = address;
        }
    }
}

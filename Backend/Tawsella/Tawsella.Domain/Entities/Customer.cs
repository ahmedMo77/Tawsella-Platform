using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.Entities
{
    public class Customer
    {
        public string Id { get; set; }
        public AppUser User { get; set; }

        public List<Order> Orders { get; set; } = new();
    }
}

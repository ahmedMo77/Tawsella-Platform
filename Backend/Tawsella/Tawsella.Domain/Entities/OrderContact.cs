using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.Entities
{
    [Owned]
    public class OrderContact
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public Location Location { get; set; }
    }
}

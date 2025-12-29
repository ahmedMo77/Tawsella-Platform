using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.Entities
{
    public class Admin
    {
        public string Id { get; set; }
        public bool IsSuperAdmin { get; set; }
        public AppUser User { get; set; }
    }
}

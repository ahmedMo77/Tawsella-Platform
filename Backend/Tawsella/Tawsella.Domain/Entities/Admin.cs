using MailKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.Entities
{
    public class Admin : BaseEntity
    {
        public bool IsSuperAdmin { get; set; }
        public AppUser User { get; set; }
    }
}

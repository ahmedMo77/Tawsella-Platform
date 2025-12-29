using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.DTOs
{
    public class CreateAdminDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsSuperAdmin { get; set; } = false;
    }
}

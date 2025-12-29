using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.DTOs
{
    public class JwtResultDto
    {
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}

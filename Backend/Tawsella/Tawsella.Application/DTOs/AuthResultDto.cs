using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tawsella.Application.DTOs
{
    public class AuthResultDto
    {
        public string Message { get; set; }
        public bool Successed { get; set; }
        public bool IsAuth { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Token { get; set; }
        public DateTime ExpireOn { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpireOn { get; set; }
        public List<string>? Roles { get; set; }
    }
}

using MailKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.DTOs
{
    public class RegisterMerchantDto : RegisterUserDto
    {
        public string BusinessName { get; set; }
        public string? BusinessAddress { get; set; }
        public string BusinessCategory { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.DTOs
{
    public class ChangePasswordDto
    {
        [Required]
        public string userId { get; set; }
        [Required]
        public string oldPassword { get; set; }
        [Required]
        public string newPassword { get; set; }
    }
}

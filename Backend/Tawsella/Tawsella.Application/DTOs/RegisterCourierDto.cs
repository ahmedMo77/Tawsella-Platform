using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.DTOs
{
    public class RegisterCourierDto : RegisterUserDto
    {
        public string NationalId { get; set; }
        public VehicleType VehicleType { get; set; }
        public string? LicensePlate { get; set; }
    }
}

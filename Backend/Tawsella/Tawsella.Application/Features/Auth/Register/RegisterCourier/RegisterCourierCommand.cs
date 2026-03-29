using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.DTOs;

namespace Tawsella.Application.Features.Auth.Register.RegisterCourier
{
    public record RegisterCourierCommand(
        string FullName,
        string Email,
        string Password,
        string PhoneNumber,
        string NationalId,
        string VehicleType,
        string VehiclePlateNumber,
        string LicenseNumber,
        DateTime LicenseExpiryDate
    ) : IRequest<BaseToReturnDto>;
}

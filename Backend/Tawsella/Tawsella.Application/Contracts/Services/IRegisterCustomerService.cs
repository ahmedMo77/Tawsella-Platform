using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.AuthDTOS;

namespace Tawsella.Application.Contracts.Services
{
    public interface IRegisterCustomerService
    {
        Task<AuthResultDto> RegisterCustomerAsync(RegisterUserDto registerDto, CancellationToken ct);
    }
}

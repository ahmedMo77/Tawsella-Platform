using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Application.Features.Admin.Commands.CreateAdmin;

namespace Tawsella.Application.Contracts.Services
{
    public interface IAuthService
    {
        Task<CreateAdminResponseDto> CreateAdminUserAsync(CreateAdminDto dto, CancellationToken ct);
        Task<CreateCourierResponseDto> RegisterCourierAsync(RegisterCourierDto registerDto, CancellationToken ct);
    }
}

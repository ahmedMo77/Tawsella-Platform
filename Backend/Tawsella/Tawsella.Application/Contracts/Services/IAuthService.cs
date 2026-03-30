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
        Task<AuthResultDto> RegisterCustomerAsync(RegisterUserDto registerDto, CancellationToken ct);
        Task<AuthResultDto> LoginAsync(LoginDto loginDto, CancellationToken ct);

        Task<BaseToReturnDto> ChangePasswordAsync(ChangePasswordDto dto, CancellationToken ct);
        Task<BaseToReturnDto> ForgotPasswordAsync(string email, CancellationToken ct);
        Task<BaseToReturnDto> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct);
        Task<BaseToReturnDto> ConfirmEmailAsync(ConfirmEmailDto dto, CancellationToken ct);
    }
}

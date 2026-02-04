using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Domain.DTOs;

namespace Tawsella.Application.Interfaces
{
    public interface IAuthService
    {
        Task<BaseToReturnDto> CreateAdminAsync(CreateAdminDto dto);
        Task<BaseToReturnDto> RegisterCustomerAsync(RegisterUserDto dto);
        Task<BaseToReturnDto> RegisterCourierAsync(RegisterCourierDto dto);
        Task<BaseToReturnDto> ApproveCourierAsync(string courierId);

        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task LogoutAsync(string refreshToken);

        Task<BaseToReturnDto> ConfirmEmailAsync(ConfirmEmailDto dto);
        Task<BaseToReturnDto> ChangePasswordAsync(ChangePasswordDto dto);
        Task<BaseToReturnDto> ForgotPasswordAsync(string email);
        Task<BaseToReturnDto> ResetPasswordAsync(ResetPasswordDto dto);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;

namespace Tawsella.Application.Interfaces
{
    public interface IAuthService
    {
        Task<BaseToReturnDto> RegisterCustomerAsync(RegisterUserDto dto);
        Task<BaseToReturnDto> RegisterCourierAsync(RegisterCourierDto dto);
        Task<BaseToReturnDto> RegisterMerchantAsync(RegisterMerchantDto dto);
        Task<BaseToReturnDto> CreateAdminAsync(CreateAdminDto dto);

        Task<BaseToReturnDto> ApproveCourierAsync(string courierId);
        Task<BaseToReturnDto> ApproveMerchantAsync(string merchantId);

        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task LogoutAsync(string refreshToken);

        Task<BaseToReturnDto> ConfirmEmailAsync(ConfirmEmailDto dto);
        Task<BaseToReturnDto> ChangePasswordAsync(ChangePasswordDto dto);
        Task<BaseToReturnDto> ForgotPasswordAsync(string email);
        Task<BaseToReturnDto> ResetPasswordAsync(ResetPasswordDto dto);

    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.Entities;

namespace Tawsella.Application.Features.Auth.Password.ChangePassword
{
    public class ChangePasswordCommandHandler
    {
            private readonly UserManager<AppUser> _userManager;

        public ChangePasswordCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<BaseToReturnDto> Handle(ChangePasswordCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return new BaseToReturnDto { Message = "User not found" };

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (!result.Succeeded)
                return new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            await _userManager.UpdateSecurityStampAsync(user);
            return new BaseToReturnDto { Success = true, Message = "Password changed successfully" };
        }
    }
}
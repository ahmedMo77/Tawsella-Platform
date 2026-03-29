using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;
using Tawsella.Domain.Entities;

namespace Tawsella.Application.Features.Auth.Password.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, BaseToReturnDto>
    {
        private readonly UserManager<AppUser> _userManager;
        public ResetPasswordCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<BaseToReturnDto> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return new BaseToReturnDto { Message = "Invalid request" };

            var result = await _userManager.ResetPasswordAsync(user, request.Code, request.NewPassword);

            if (!result.Succeeded)
                return new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

           await _userManager.UpdateSecurityStampAsync(user);
           return new BaseToReturnDto { Success = true, Message = "Password reset successfully" };
        }
    }
}

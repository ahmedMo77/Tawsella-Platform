using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;
using Tawsella.Application.Entities;

namespace Tawsella.Application.Features.Auth.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, BaseToReturnDto>
    {
        private readonly UserManager<AppUser> _userManager;

        public ConfirmEmailCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<BaseToReturnDto> Handle(ConfirmEmailCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new BaseToReturnDto { Message = "User not found" };

            if (await _userManager.IsEmailConfirmedAsync(user))
                return new BaseToReturnDto { Success = true, Message = "Email is already confirmed." };

            var result = await _userManager.ConfirmEmailAsync(user, request.Code);

            if (!result.Succeeded) 
                return new BaseToReturnDto { Message = "Invalid or expired code." };

            return new BaseToReturnDto { Success = true, Message = "Email confirmed successfully." };
        }
    }
}

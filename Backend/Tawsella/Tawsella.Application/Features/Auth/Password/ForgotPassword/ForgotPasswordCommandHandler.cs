using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Services;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.Entities;

namespace Tawsella.Application.Features.Auth.Password.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, BaseToReturnDto>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordCommandHandler(UserManager<AppUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<BaseToReturnDto> Handle(ForgotPasswordCommand request, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new BaseToReturnDto { Success = true, Message = "If the email exists, a code has been sent." };

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailSender.SendEmailAsync(user.Email, "Reset Password Code", $"Your code is: {code}");

            return new BaseToReturnDto { Success = true, Message = "Reset code sent to your email." };
        }
    }
}

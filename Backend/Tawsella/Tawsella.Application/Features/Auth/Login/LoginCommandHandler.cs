using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Application.Entities;
using Tawsella.Application.Enums;

namespace Tawsella.Application.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ICourierRepository _courierRepo; // حقن الريبو مباشرة

        public LoginCommandHandler(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            ICourierRepository courierRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _courierRepo = courierRepo;
        }

        public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new AuthResultDto { Message = "Invalid email or password" };

            if (await _userManager.IsInRoleAsync(user, Roles.Courier.ToString()))
            {
                var courier = await _courierRepo.GetByIdAsync(user.Id, cancellationToken);
                if (courier != null && !courier.IsApproved)
                {
                    return new AuthResultDto { Message = "Your account is still under review by the admin." };
                }
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return new AuthResultDto { Message = "Please confirm your email first." };

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return new AuthResultDto { Message = "Invalid email or password" };

            return await _tokenService.GenerateTokensPairAsync(user);
        }
    }
}
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Authentication.Queries.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginQueryResponse>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public LoginQueryHandler(
            ICourierRepository courierRepository,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _courierRepository = courierRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<LoginQueryResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new LoginQueryResponse
                {
                    Message = "Invalid email or password"
                };
            }

            var isCourier = await _userManager.IsInRoleAsync(user, Roles.Courier.ToString());

            if (isCourier)
            {
                var courier = await _courierRepository.GetByIdAsync(user.Id, cancellationToken);
                if (courier != null && !courier.IsApproved)
                {
                    return new LoginQueryResponse
                    {
                        Message = "Your account is still under review by the admin."
                    };
                }
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new LoginQueryResponse
                {
                    Message = "Please confirm your email first."
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded)
            {
                return new LoginQueryResponse
                {
                    Message = "Invalid email or password"
                };
            }

            var authResult = await _tokenService.GenerateTokensPairAsync(user);

            return new LoginQueryResponse
            {
                Success = authResult.Success,
                IsAuth = authResult.IsAuth,
                UserName = authResult.UserName,
                Email = authResult.Email,
                Token = authResult.Token,
                ExpireOn = authResult.ExpireOn,
                RefreshToken = authResult.RefreshToken,
                RefreshTokenExpireOn = authResult.RefreshTokenExpireOn,
                Roles = authResult.Roles
            };
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Metrics;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ICourierRepository _courierRepo;


        public AuthService(
            UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            SignInManager<AppUser> signInManager, 
            ITokenService tokenService, 
            IEmailService emailService, 
            ICourierRepository courierRepo)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _courierRepo = courierRepo;
            _emailService = emailService;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Please confirm your email first."
                };
            }

            var passwordResult = await _signInManager.CheckPasswordSignInAsync(
                user, loginDto.Password,
                lockoutOnFailure: false);

            if (!passwordResult.Succeeded)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            if (await _userManager.IsInRoleAsync(user, Roles.Courier.ToString()))
            {
                var courier = await _courierRepo.GetByIdAsync(user.Id, ct);

                if (courier == null)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Courier account is invalid."
                    };
                }

                if (!courier.IsApproved)
                {
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = "Your account is still under review."
                    };
                }
            }

            return await _tokenService.GenerateTokensPairAsync(user);
        }

        public async Task<BaseToReturnDto> ChangePasswordAsync(ChangePasswordDto dto, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return new BaseToReturnDto { Message = "User not found" };

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (!result.Succeeded) 
                return new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            await _userManager.UpdateSecurityStampAsync(user);
            return new BaseToReturnDto { Success = true, Message = "Password changed successfully" };
        }

        public async Task<BaseToReturnDto> ForgotPasswordAsync(string email, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return new BaseToReturnDto { Success = true, Message = "If the email exists, a code has been sent." };

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendEmailAsync(user.Email, "Reset Password Code", $"Your code is: {code}");

            return new BaseToReturnDto { Success = true, Message = "Reset code sent to your email." };
        }

        public async Task<BaseToReturnDto> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return new BaseToReturnDto { Message = "User not found" };

            var result = await _userManager.ResetPasswordAsync(user, dto.Code, dto.NewPassword);
            if (!result.Succeeded) 
                return new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };
            await _userManager.UpdateSecurityStampAsync(user);
            return new BaseToReturnDto { Success = true, Message = "Password reset successfully" };
        }

        public async Task<BaseToReturnDto> ConfirmEmailAsync(ConfirmEmailDto dto, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return new BaseToReturnDto { Message = "User not found" };

            if (await _userManager.IsEmailConfirmedAsync(user))
                return new BaseToReturnDto { Success = true, Message = "Email is already confirmed." };

            var result = await _userManager.ConfirmEmailAsync(user, dto.Code);
            if (!result.Succeeded)
                return new BaseToReturnDto { Message = "Invalid or expired code." };
            
            return new BaseToReturnDto { Success = true, Message = "Email confirmed successfully." };
        }

    }
}

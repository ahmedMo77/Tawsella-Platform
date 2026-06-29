using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly AppDbContext _dbContext;
        private readonly IAsyncRepository<Customer> _customerRepo;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<Courier> _courierRepo;

        public AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IEmailService emailService, AppDbContext dbContext, IAsyncRepository<Customer> customerRepo, ILogger<AuthService> logger, IMapper mapper, IAsyncRepository<Courier> courierRepo)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _dbContext = dbContext;
            _customerRepo = customerRepo;
            _logger = logger;
            _mapper = mapper;
            _courierRepo = courierRepo;
        }

        public async Task<CreateAdminResponseDto> CreateAdminUserAsync(CreateAdminDto admin, CancellationToken ct)
        {
            if (await _userManager.FindByEmailAsync(admin.Email) != null)
                return new CreateAdminResponseDto { Success = false, Message = "Email already in use" };

            var user = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                FullName = admin.FullName,
                UserName = admin.Email.Split('@')[0],
                Email = admin.Email,
                EmailConfirmed = true
            };

            var tempPassword = GenerateTempPassword();

            var result = await _userManager.CreateAsync(user, tempPassword);
            if (!result.Succeeded)
                return new CreateAdminResponseDto { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            var roleName = admin.IsSuperAdmin ? Roles.SuperAdmin.ToString() : Roles.Admin.ToString();

            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(user, roleName);

            return new CreateAdminResponseDto { Success = true, Message = "Admin user created successfully", Id = user.Id, Password = tempPassword };
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto loginDto, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return new AuthResultDto { Success = false, Message = "Invalid email or password" };

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return new AuthResultDto { Success = false, Message = "Please confirm your email first." };

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return new AuthResultDto { Success = false, Message = "Invalid email or password" };

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

        private string GenerateTempPassword() => $"Tawsella@{Guid.NewGuid():N}".Substring(0, 12);
    }
}

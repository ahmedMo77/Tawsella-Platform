using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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

        public async Task<CreateCourierResponseDto> RegisterCourierAsync(RegisterCourierDto registerDto, CancellationToken ct)
        {
            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
                return new CreateCourierResponseDto { Success = false, Message = "Email already in use" };

            var user = new AppUser
            {
                FullName = registerDto.FullName,
                UserName = registerDto.Email.Split('@')[0],
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return new CreateCourierResponseDto { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            var role = Roles.Courier.ToString();

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            return new CreateCourierResponseDto { Success = true, Id = user.Id };
        }

        private string GenerateTempPassword() => $"Temp@{Guid.NewGuid():N}".Substring(0, 12);
    }
}

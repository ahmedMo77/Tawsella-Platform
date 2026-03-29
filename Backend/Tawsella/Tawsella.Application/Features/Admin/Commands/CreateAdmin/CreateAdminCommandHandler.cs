using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;


namespace Tawsella.Application.Features.Admin.Commands.CreateAdmin
{
    public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, BaseToReturnDto>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAdminRepository _adminRepo;
        private readonly IEmailSender _emailService;

        public CreateAdminCommandHandler(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAdminRepository adminRepo,
            IEmailSender emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _adminRepo = adminRepo;
            _emailService = emailService;
        }
        public async Task<BaseToReturnDto> Handle(CreateAdminCommand request, CancellationToken ct)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return new BaseToReturnDto { Message = "Email already in use" };

            var password = GenerateTempPassword();
            var user = new AppUser
            {
                FullName = request.FullName,
                UserName = request.Email.Split('@')[0],
                Email = request.Email,
                EmailConfirmed = true
            };

            var roleName = request.IsSuperAdmin ? 
                Roles.SuperAdmin.ToString() : Roles.Admin.ToString();

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(user, roleName);

            var adminEntity = new Domain.Entities.Admin
            {
                Id = user.Id,
                IsSuperAdmin = request.IsSuperAdmin
            };

            await _adminRepo.AddAsync(adminEntity, ct);

            await SendInvitationEmail(request.Email, request.FullName, password);

            return new BaseToReturnDto { Success = true, Message = "Admin created and invitation sent successfully." };
        }
        private string GenerateTempPassword() => $"Temp@{Guid.NewGuid():N}".Substring(0, 12);
        private async Task SendInvitationEmail(string email, string name, string password)
        {
            string body = $@"
            <div style='font-family: sans-serif; max-width: 600px; border: 1px solid #eee; padding: 20px;'>
                <h2 style='color: #2c3e50;'>Welcome to Tawsella Team!</h2>
                <p>Hello <strong>{name}</strong>,</p>
                <p>A new administrator account has been created for you.</p>
                <div style='background: #f4f7f6; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                    <p><strong>Email:</strong> {email}</p>
                    <p><strong>Temp Password:</strong> <span style='color: #e74c3c; font-weight: bold;'>{password}</span></p>
                </div>
                <p>Please change your password after your first login.</p>
            </div>";

            await _emailService.SendEmailAsync(email, "Admin Invitation", body);
        }
    }
}
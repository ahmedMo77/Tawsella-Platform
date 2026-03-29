using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs;
using Tawsella.Application.Entities;
using Tawsella.Application.Enums;

namespace Tawsella.Application.Features.Auth.Register.RegisterCourier
{
    public class RegisterCourierCommandHandler : IRequestHandler<RegisterCourierCommand, BaseToReturnDto>
    {
        private readonly ICourierRepository _repo;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterCourierCommandHandler(
            ICourierRepository repo,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _repo = repo;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<BaseToReturnDto> Handle(RegisterCourierCommand request, CancellationToken ct)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email) != null;
            var nationalIdExists = await _repo.GetCourierByNationalIdAsync(request.NationalId, ct);

            if (userExists || nationalIdExists == null)
                return new BaseToReturnDto { Message = "Email or National ID already in use" };

            var user = new AppUser
            {
                FullName = request.FullName,
                UserName = request.Email.Split('@')[0],
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            var roleName = Roles.Courier.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(user, roleName);

            var courier = new Courier
            {
                Id = user.Id, // الربط مع Identity
                IsApproved = false, // ينتظر مراجعة الأدمن
                NationalId = request.NationalId,
                VehicleType = request.VehicleType,
                VehiclePlateNumber = request.VehiclePlateNumber,
                LicenseNumber = request.LicenseNumber,
                LicenseExpiryDate = request.LicenseExpiryDate,
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(courier);

            return new BaseToReturnDto { Success = true, Message = "Your request is under review" };
        }
    }
}
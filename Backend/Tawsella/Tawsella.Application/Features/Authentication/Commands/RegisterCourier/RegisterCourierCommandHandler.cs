using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tawsella.Application.Contracts;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Authentication.Commands.RegisterCourier
{
    public class RegisterCourierCommandHandler 
        : IRequestHandler<RegisterCourierCommand, RegisterCourierCommandResponse>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterCourierCommandHandler(
            ICourierRepository courierRepository,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _courierRepository = courierRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<RegisterCourierCommandResponse> Handle(
            RegisterCourierCommand request,
            CancellationToken cancellationToken)
        {
            var courierWithNationalId = await _courierRepository.GetCourierByNationalIdAsync(request.NationalId, cancellationToken);

            if (await _userManager.FindByEmailAsync(request.Email) != null || courierWithNationalId != null)
            {
                return new RegisterCourierCommandResponse
                {
                    Message = "Email or Id already in use"
                };
            }

            var user = new AppUser
            {
                FullName = request.FullName,
                UserName = request.Email.Substring(0, request.Email.IndexOf('@')),
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new RegisterCourierCommandResponse
                {
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            var roleName = Roles.Courier.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(user, Roles.Courier.ToString());

            var courier = new Courier
            {
                Id = user.Id,
                IsApproved = false,
                User = user,
                NationalId = request.NationalId,
                VehicleType = request.VehicleType,
                VehiclePlateNumber = request.VehiclePlateNumber,
                LicenseNumber = request.LicenseNumber,
                LicenseExpiryDate = request.LicenseExpiryDate,
                CreatedAt = DateTime.UtcNow
            };

            await _courierRepository.AddAsync(courier, cancellationToken);

            return new RegisterCourierCommandResponse
            {
                Success = true,
                Message = "Your request is under review"
            };
        }
    }
}

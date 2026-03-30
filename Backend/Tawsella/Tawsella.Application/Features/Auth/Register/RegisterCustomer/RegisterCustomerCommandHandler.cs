using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Auth.Register.RegisterCustomer
{
    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, BaseToReturnDto>
    {
        private readonly ICustomerRepository _repo;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public RegisterCustomerCommandHandler(
            ICustomerRepository repo,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService) // ضفنا الإيميل سيرفيس هنا
        {
            _repo = repo;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        public async Task<BaseToReturnDto> Handle(RegisterCustomerCommand request, CancellationToken ct)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return new BaseToReturnDto { Message = "Email already in use" };

            var user = new AppUser
            {
                FullName = request.FullName,
                UserName = request.Email.Split('@')[0],
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            var roleName = Roles.Customer.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(user, roleName);

            var customer = new Customer
            {
                Id = user.Id,
                User = user,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(customer);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // داخل ميثود RegisterCustomerAsync
            string verifyHtmlBody = $@"
                <h2 style='color: #2c3e50;'>Verify Your Email</h2>
                <p>Thank you for signing up! Please use the following code to complete your registration:</p>
                <div style='background: #f4f4f4; padding: 20px; font-size: 32px; font-weight: bold; text-align: center; letter-spacing: 10px; color: #2c3e50; border: 1px dashed #2c3e50;'>
                    {code}
                </div>
                <p style='color: #7f8c8d; font-size: 14px; margin-top: 20px;'>This code will expire in 15 minutes.</p>";

            await _emailService.SendEmailAsync(user.Email, "Confirm Your Email", verifyHtmlBody);
            return new BaseToReturnDto { Success = true, Message = "Registration successful. Please verify your email." };
        }
    }
}
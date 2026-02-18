using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Authentication.Commands.RegisterCustomer
{
    public class RegisterCustomerCommandHandler 
        : IRequestHandler<RegisterCustomerCommand, RegisterCustomerCommandResponse>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailService;

        public RegisterCustomerCommandHandler(
            ICustomerRepository customerRepository,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailService)
        {
            _customerRepository = customerRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        public async Task<RegisterCustomerCommandResponse> Handle(
            RegisterCustomerCommand request,
            CancellationToken cancellationToken)
        {
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new RegisterCustomerCommandResponse
                {
                    Message = "Email already in use"
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
                return new RegisterCustomerCommandResponse
                {
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            var roleName = Roles.Customer.ToString();

            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(user, Roles.Customer.ToString());

            var customer = new Customer
            {
                Id = user.Id,
                User = user,
                CreatedAt = DateTime.UtcNow
            };
            await _customerRepository.AddAsync(customer, cancellationToken);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string verifyHtmlBody = $@"
                <h2 style='color: #2c3e50;'>Verify Your Email</h2>
                <p>Thank you for signing up! Please use the following code to complete your registration:</p>
                <div style='background: #f4f4f4; padding: 20px; font-size: 32px; font-weight: bold; text-align: center; letter-spacing: 10px; color: #2c3e50; border: 1px dashed #2c3e50;'>
                    {code}
                </div>
                <p style='color: #7f8c8d; font-size: 14px; margin-top: 20px;'>This code will expire in 15 minutes.</p>";

            await _emailService.SendEmailAsync(user.Email, "Confirm Your Email", verifyHtmlBody);

            return new RegisterCustomerCommandResponse
            {
                Success = true,
                Message = "Registration successful. Please verify your email."
            };
        }
    }
}

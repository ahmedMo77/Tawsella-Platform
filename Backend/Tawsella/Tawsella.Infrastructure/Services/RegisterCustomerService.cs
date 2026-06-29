using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Infrastructure.Services
{
    public class RegisterCustomerService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _dbContext;
        private readonly IAsyncRepository<Customer> _customerRepo;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;

        public RegisterCustomerService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IEmailService emailService, AppDbContext dbContext, IAsyncRepository<Customer> customerRepo, ILogger<AuthService> logger, IMapper mapper, IAsyncRepository<Courier> courierRepo)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _dbContext = dbContext;
            _customerRepo = customerRepo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AuthResultDto> RegisterCustomerAsync(RegisterUserDto registerDto, CancellationToken ct)
        {
            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Message = "Email already in use."
                };
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var user = new AppUser
                {
                    FullName = registerDto.FullName,
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (!result.Succeeded)
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
                    };

                var roleName = Roles.Customer.ToString();

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

                    if (!createRoleResult.Succeeded)
                        return new AuthResultDto
                        {
                            Success = false,
                            Message = "Failed to create customer role."
                        };
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);

                if (!addRoleResult.Succeeded)
                    return new AuthResultDto
                    {
                        Success = false,
                        Message = string.Join(", ", addRoleResult.Errors.Select(e => e.Description))
                    };

                var customer = _mapper.Map<Customer>(registerDto);
                customer.Id = user.Id;

                await _customerRepo.AddAsync(customer, ct);

                await transaction.CommitAsync(ct);

                try
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    await _emailService.SendVerificationCodeAsync(
                        user.Email!,
                        token,
                        ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed sending verification email to {Email}",
                        user.Email);
                }

                return new AuthResultDto
                {
                    Success = true,
                    Id = user.Id,
                    Message = "Registration completed successfully. Please verify your email."
                };
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}

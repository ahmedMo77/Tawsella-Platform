using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailService; 

        public AuthService(AppDbContext context, UserManager<AppUser> userManager,
            ITokenService tokenService, SignInManager<AppUser> signInManager, 
            RoleManager<IdentityRole> roleManger, IEmailSender emailService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManger;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<BaseToReturnDto> CreateAdminAsync(CreateAdminDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return new BaseToReturnDto { Message = "Email already in use" };

            var password = GenerateTempPassword();

            var user = new AppUser
            {
                FullName = dto.FullName,
                UserName = dto.Email.Substring(0, dto.Email.IndexOf('@')),
                Email = dto.Email,
                EmailConfirmed = true
            };
            
            var role = dto.IsSuperAdmin ? Roles.SuperAdmin.ToString() : Roles.Admin.ToString();

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            var admin = new Admin
            {
                Id = user.Id,
                User = user,
                IsSuperAdmin = dto.IsSuperAdmin
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            string body = $@"
                <h2 style='color: #2c3e50;'>Welcome to Tawsella Team!</h2>
                <p>Hello <strong>{dto.FullName}</strong>,</p>
                <p>A new administrator account has been created for you. You can now access the dashboard using the following credentials:</p>
                <div style='background: #f4f7f6; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                    <p style='margin: 0;'><strong>Email:</strong> {dto.Email}</p>
                    <p style='margin: 10px 0 0 0;'><strong>Temporary Password:</strong> <span style='color: #e74c3c; font-family: monospace; font-weight: bold;'>{password}</span></p>
                </div>
                <p>Please log in and change your password immediately to secure your account.</p>
                <div style='text-align: center; margin-top: 30px;'>
                    <a href='#' style='background: #3498db; color: white; padding: 12px 25px; text-decoration: none; border-radius: 4px; font-weight: bold;'>Go to Dashboard</a>
                </div>";

            await _emailService.SendEmailAsync(dto.Email, "Admin Invitation", body);

            return new BaseToReturnDto { Success = true ,Message = "Admin created and invitation sent successfully. Account is active." };
        }
        public async Task<BaseToReturnDto> RegisterCustomerAsync(RegisterUserDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return new BaseToReturnDto { Message = "Email already in use" };

            var user = new AppUser
            {
                FullName = dto.FullName,
                UserName = dto.Email.Substring(0, dto.Email.IndexOf('@')),
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };


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
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

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
        public async Task<BaseToReturnDto> RegisterCourierAsync(RegisterCourierDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null || await _context.Couriers.FirstOrDefaultAsync(c => c.NationalId == dto.NationalId) != null)
                return new BaseToReturnDto { Message = "Email or Id already in use" };

            var user = new AppUser
            {
                FullName = dto.FullName,
                UserName = dto.Email.Substring(0, dto.Email.IndexOf('@')),
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };


            var roleName = Roles.Courier.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(user, Roles.Courier.ToString());

            var courier = new Courier
            {
                Id = user.Id,
                IsApproved = false,
                User = user,
                NationalId = dto.NationalId,
                VehicleType = dto.VehicleType,
                VehiclePlateNumber = dto.VehiclePlateNumber,
                LicenseNumber = dto.LicenseNumber,
                LicenseExpiryDate = dto.LicenseExpiryDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Couriers.Add(courier);
            await _context.SaveChangesAsync();

            return new BaseToReturnDto { Success = true, Message = "Your request is under review" };
        }
        public async Task<BaseToReturnDto> RegisterMerchantAsync(RegisterMerchantDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return new BaseToReturnDto { Message = "Email already in use" };

            var user = new AppUser
            {
                FullName = dto.FullName,
                UserName = dto.Email.Substring(0, dto.Email.IndexOf('@')),
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };


            var roleName = Roles.Merchant.ToString();

            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(user, Roles.Merchant.ToString());

            var merchant = new Merchant
            {
                Id = user.Id,
                IsApproved = false,
                User = user,
                BusinessName = dto.BusinessName,
                BusinessRegistrationNumber = dto.BusinessRegistrationNumber,
                BusinessAddress = dto.BusinessAddress,
                BusinessCategory = dto.BusinessCategory,
                CreatedAt = DateTime.UtcNow
            };

            _context.Merchants.Add(merchant);
            await _context.SaveChangesAsync();

            return new BaseToReturnDto { Success = true, Message = "Your request is under review" };
        }

        public async Task<BaseToReturnDto> ApproveCourierAsync(string courierId)
        {
            var courier = await _context.Couriers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == courierId);

            if (courier == null || courier.IsApproved)
                return new BaseToReturnDto { Message = "This Courier is already approved or not found." };

            courier.IsApproved = true;
            courier.User.EmailConfirmed = true;
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(courier.User.Email, "Account Approved", "Congratulations! Your account is now active. You can login now.");

            return new BaseToReturnDto { Success = true, Message = "Approved Successfully." };
        }
        public async Task<BaseToReturnDto> ApproveMerchantAsync(string merchantId)
        {
            var merchant = await _context.Merchants
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == merchantId);

            if (merchant == null || merchant.IsApproved)
                return new BaseToReturnDto { Message = "This Merchant is already approved or not found." };

            merchant.IsApproved = true;
            merchant.User.EmailConfirmed = true;
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(merchant.User.Email, "Account Approved", "Congratulations! Your account is now active. You can login now.");

            return new BaseToReturnDto { Success = true, Message = "Approved Successfully." };
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return new AuthResultDto { Message = "Invalid email or password" };

            var isCourier = await _userManager.IsInRoleAsync(user, Roles.Courier.ToString());
            var isMerchant = await _userManager.IsInRoleAsync(user, Roles.Merchant.ToString());

            if (isCourier)
            {
                var courier = await _context.Couriers.FirstOrDefaultAsync(c => c.Id == user.Id);
                if (courier != null && !courier.IsApproved)
                    return new AuthResultDto { Message = "Your account is still under review by the admin." };
            }

            if (isMerchant)
            {
                var merchant = await _context.Merchants.FirstOrDefaultAsync(m => m.Id == user.Id);
                if (merchant != null && !merchant.IsApproved)
                    return new AuthResultDto { Message = "Your merchant account is still under review." };
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return new AuthResultDto { Message = "Please confirm your email first." };

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
                return new AuthResultDto { Message = "Invalid email or password" };

            return await _tokenService.GenerateTokensPairAsync(user);
        }
        public async Task LogoutAsync(string refreshToken) =>
            await _tokenService.RevokeRefreshTokenAsync(refreshToken);

        public async Task<BaseToReturnDto> ConfirmEmailAsync(ConfirmEmailDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.email);
            if (user == null) return new BaseToReturnDto { Message = "User not found" };

            var result = await _userManager.ConfirmEmailAsync(user, dto.code);
            return result.Succeeded
                ? new BaseToReturnDto { Success = true, Message = "Email confirmed successfully" }
                : new BaseToReturnDto { Message = "Invalid or expired code" };
        }
        public async Task<BaseToReturnDto> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.userId);
            if (user == null) return new BaseToReturnDto { Message = "User not found" };

            var result = await _userManager.ChangePasswordAsync(user, dto.oldPassword, dto.newPassword);
            await _userManager.UpdateSecurityStampAsync(user);

            return result.Succeeded
                ? new BaseToReturnDto { Success = true, Message = "Password changed successfully" }
                : new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };
        }
        public async Task<BaseToReturnDto> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return new BaseToReturnDto { Message = "Invalid request" };

            var result = await _userManager.ResetPasswordAsync(user, dto.Code, dto.NewPassword);
            await _userManager.UpdateSecurityStampAsync(user);

            return result.Succeeded
                ? new BaseToReturnDto { Success = true, Message = "Password reset successfully" }
                : new BaseToReturnDto { Message = string.Join(", ", result.Errors.Select(e => e.Description)) };
        }
        public async Task<BaseToReturnDto> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new BaseToReturnDto { Success = true, Message = "If the email exists, a code has been sent." };

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendEmailAsync(user.Email, "Reset Password Code", $"Your verification code is: {code}");

            return new BaseToReturnDto { Success = true, Message = "Reset code sent to your email." };
        }

        private string GenerateTempPassword()
        {
            return $"Temp@{Guid.NewGuid():N}".Substring(0, 12);
        }
    }
}

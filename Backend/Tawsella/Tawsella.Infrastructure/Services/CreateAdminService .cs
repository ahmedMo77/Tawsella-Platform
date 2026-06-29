using Microsoft.AspNetCore.Identity;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Infrastructure.Services
{
    public class CreateAdminService : ICreateAdminService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _dbContext;
        private readonly IAdminRepository _adminRepo;


        public CreateAdminService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext dbContext,
            IAdminRepository adminRepo)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _adminRepo = adminRepo;
        }

        public async Task<CreateAdminResponseDto> CreateAdminAsync(CreateAdminDto admin, CancellationToken ct)
        {
            if (await _userManager.FindByEmailAsync(admin.Email) != null)
            {
                return new CreateAdminResponseDto
                {
                    Success = false,
                    Message = "Email already in use."
                };
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var tempPassword = GenerateTempPassword();

                var user = new AppUser
                {
                    Id = Guid.NewGuid().ToString(),
                    FullName = admin.FullName,
                    UserName = admin.Email,
                    Email = admin.Email,
                    EmailConfirmed = true
                };

                var createUserResult = await _userManager.CreateAsync(user, tempPassword);

                if (!createUserResult.Succeeded)
                {
                    return new CreateAdminResponseDto
                    {
                        Success = false,
                        Message = string.Join(", ",
                            createUserResult.Errors.Select(e => e.Description))
                    };
                }

                var roleName = admin.IsSuperAdmin
                    ? Roles.SuperAdmin.ToString()
                    : Roles.Admin.ToString();

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var createRoleResult =
                        await _roleManager.CreateAsync(new IdentityRole(roleName));

                    if (!createRoleResult.Succeeded)
                    {
                        return new CreateAdminResponseDto
                        {
                            Success = false,
                            Message = "Failed to create admin role."
                        };
                    }
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);

                if (!addRoleResult.Succeeded)
                {
                    return new CreateAdminResponseDto
                    {
                        Success = false,
                        Message = string.Join(", ",
                            addRoleResult.Errors.Select(e => e.Description))
                    };
                }

                await _adminRepo.AddAsync(new Admin
                {
                    Id = user.Id,
                    IsSuperAdmin = admin.IsSuperAdmin
                }, ct);

                await transaction.CommitAsync(ct);

                return new CreateAdminResponseDto
                {
                    Success = true,
                    Id = user.Id,
                    Password = tempPassword,
                    Message = "Admin created successfully."
                };

            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }

        private string GenerateTempPassword() => $"Tawsella@{Guid.NewGuid():N}".Substring(0, 12);

    }
}

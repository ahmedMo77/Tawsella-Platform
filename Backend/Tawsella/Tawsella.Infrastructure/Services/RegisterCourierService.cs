using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Infrastructure.Services
{
    public class RegisterCourierService : IRegisterCourierService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICourierRepository _courierRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public RegisterCourierService(
            RoleManager<IdentityRole> roleManager,
            UserManager<AppUser> userManager,
            ICourierRepository courierRepo,
            AppDbContext dbContext,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _mapper = mapper;
            _courierRepo = courierRepo;
        }
        public async Task<CreateCourierResponseDto> RegisterCourierAsync(RegisterCourierDto registerDto, CancellationToken ct)
        {
            if (await _userManager.FindByEmailAsync(registerDto.Email) != null)
                return new CreateCourierResponseDto
                {
                    Success = false,
                    Message = "Email already in use"
                };

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
                    return new CreateCourierResponseDto
                    {
                        Success = false,
                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
                    };

                var roleName = Roles.Courier.ToString();

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

                    if (!createRoleResult.Succeeded)
                    {
                        await transaction.RollbackAsync(ct);

                        return new CreateCourierResponseDto
                        {
                            Success = false,
                            Message = "Failed to create courier role."
                        };
                    }
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);

                if (!addRoleResult.Succeeded)
                    return new CreateCourierResponseDto
                    {
                        Success = false,
                        Message = string.Join(", ", addRoleResult.Errors.Select(e => e.Description))
                    };

                var courier = _mapper.Map<Courier>(registerDto);
                courier.Id = user.Id;
                courier.IsApproved = false; // Set to false until admin approval

                await _courierRepo.AddAsync(courier, ct);

                await transaction.CommitAsync(ct);

                return new CreateCourierResponseDto
                {
                    Success = true,
                    Id = user.Id,
                    Message = "Registration successful. Your application is pending admin approval."
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

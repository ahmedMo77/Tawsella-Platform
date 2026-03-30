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
using Tawsella.Application.DTOs.AuthDTOS;
using AutoMapper;


namespace Tawsella.Application.Features.Admin.Commands.CreateAdmin
{
    public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, BaseToReturnDto>
    {
        private readonly IAuthService _authService;
        private readonly IAdminRepository _adminRepo;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public CreateAdminCommandHandler(
            IAuthService authService,
            IAdminRepository adminRepo,
            IEmailService emailService,
            IMapper mapper)
        {
            _authService = authService;
            _adminRepo = adminRepo;
            _emailService = emailService;
            _mapper = mapper;
        }
        public async Task<BaseToReturnDto> Handle(CreateAdminCommand request, CancellationToken ct)
        {

            var admin = _mapper.Map<CreateAdminDto>(request);

            var result = await _authService.CreateAdminUserAsync(admin, ct);
            if (!result.Success)
                return new BaseToReturnDto { Success = false, Message = result.Message };

            await _adminRepo.AddAsync(new Domain.Entities.Admin
            {
                Id = result.Id,
                IsSuperAdmin = request.IsSuperAdmin
            }, ct);

            var adminEmailDto = _mapper.Map<CreateAdminEmailDto>(request);
            adminEmailDto.Password = result.Password;

            await _emailService.SendAdminInvitationEmail(adminEmailDto, ct);

            return new BaseToReturnDto { Success = true, Message = "Admin created and invitation sent successfully." };
        }
    }
}
using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.AuthDTOS;
using AutoMapper;


namespace Tawsella.Application.Features.Admin.Commands.CreateAdmin
{
    public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, BaseToReturnDto>
    {
        private readonly ICreateAdminService _adminService;
        private readonly IAdminRepository _adminRepo;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public CreateAdminCommandHandler(
            ICreateAdminService adminService,
            IAdminRepository adminRepo,
            IEmailService emailService,
            IMapper mapper)
        {
            _adminService = adminService;
            _adminRepo = adminRepo;
            _emailService = emailService;
            _mapper = mapper;
        }
        public async Task<BaseToReturnDto> Handle(CreateAdminCommand request, CancellationToken ct)
        {
            var dto = _mapper.Map<CreateAdminDto>(request);

            var result = await _adminService.CreateAdminAsync(dto, ct);

            if (!result.Success)
            {
                return new BaseToReturnDto
                {
                    Success = false,
                    Message = result.Message
                };
            }

            var adminEmailDto = _mapper.Map<CreateAdminEmailDto>(request);
            adminEmailDto.Password = result.Password;

            await _emailService.SendAdminInvitationEmail(adminEmailDto, ct);

            return new BaseToReturnDto
            {
                Success = true,
                Message = "Admin created successfully. Invitation email sent."
            };
        }
    }
}
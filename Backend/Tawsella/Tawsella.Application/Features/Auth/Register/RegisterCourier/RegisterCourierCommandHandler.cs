using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.AuthDTOS;

namespace Tawsella.Application.Features.Auth.Register.RegisterCourier
{
    public class RegisterCourierCommandHandler : IRequestHandler<RegisterCourierCommand, BaseToReturnDto>
    {
        private readonly ICourierRepository _courierRepo;
        private readonly IRegisterCourierServcie _courierService;
        private readonly IMapper _mapper;

        public RegisterCourierCommandHandler(ICourierRepository repo, IMapper mapper, IRegisterCourierServcie courierService)
        {
            _mapper = mapper;
            _courierRepo = repo;
            _courierService = courierService;
        }
        public async Task<BaseToReturnDto> Handle(RegisterCourierCommand request, CancellationToken ct)
        {
            var nationalIdExists = await _courierRepo.GetCourierByNationalIdAsync(request.NationalId, ct);
            if (nationalIdExists != null)
                return new BaseToReturnDto
                { 
                    Success = false,
                    Message = "National ID already in use"
                };

            var registerDto = _mapper.Map<RegisterCourierDto>(request);

            var result = await _courierService.RegisterCourierAsync(registerDto, ct);

            return new BaseToReturnDto
            {
                Success = result.Success,
                Message = result.Message
            };
        }
    }
}
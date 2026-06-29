using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.AuthDTOS;

namespace Tawsella.Application.Features.Auth.Register.RegisterCustomer
{
    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, BaseToReturnDto>
    {
        private readonly IRegisterCustomerService _customerService;
        private readonly IMapper _mapper;

        public RegisterCustomerCommandHandler(
            IRegisterCustomerService customerService,
            IMapper mapper)
        {
            _customerService = customerService;
            _mapper = mapper;
        }

        public async Task<BaseToReturnDto> Handle(RegisterCustomerCommand request, CancellationToken ct)
        {
            var registerDto = _mapper.Map<RegisterUserDto>(request);

            var result = await _customerService.RegisterCustomerAsync(registerDto, ct);

            return new BaseToReturnDto
            {
                Success = result.Success,
                Message = result.Message
            };
        }
    }
}
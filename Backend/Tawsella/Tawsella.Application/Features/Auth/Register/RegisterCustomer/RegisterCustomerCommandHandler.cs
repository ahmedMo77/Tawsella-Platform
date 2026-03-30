using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Auth.Register.RegisterCustomer
{
    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, BaseToReturnDto>
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;


        public RegisterCustomerCommandHandler(
            ICustomerRepository customerRepo,
            IAuthService authService,
            IMapper mapper)
        {
            _customerRepo = customerRepo;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<BaseToReturnDto> Handle(RegisterCustomerCommand request, CancellationToken ct)
        {
            var registerDto = _mapper.Map<RegisterUserDto>(request);

            var authResult = await _authService.RegisterCustomerAsync(registerDto, ct);

            if (!authResult.Success)
                return new BaseToReturnDto { Success = false, Message = authResult.Message };

            var customer = _mapper.Map<Customer>(request);
            customer.Id = authResult.Id;

            await _customerRepo.AddAsync(customer, ct);

            return new BaseToReturnDto
            {
                Success = authResult.Success,
                Message = authResult.Message
            };
        }
    }
}
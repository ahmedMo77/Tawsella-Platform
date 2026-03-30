using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using AutoMapper;

namespace Tawsella.Application.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResultDto>
    {
        private readonly IAuthService _authService;
        private readonly ICourierRepository _courierRepo;
        private readonly IMapper _mapper;

        public LoginCommandHandler(
            IAuthService authService,
            IMapper mapper,
            ITokenService tokenService,
            ICourierRepository courierRepo)
        {
            _authService = authService;
            _courierRepo = courierRepo;
            _mapper = mapper;
        }

        public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken ct)
        {
            var loginDto = _mapper.Map<LoginDto>(request);

            var authResult = await _authService.LoginAsync(loginDto, ct);

            if (!authResult.Success) return authResult;

            if (authResult.Roles.Contains(Roles.Courier.ToString()))
            {
                var courier = await _courierRepo.GetByIdAsync(authResult.Id, ct);
                if (courier != null && !courier.IsApproved)
                {
                    return new AuthResultDto { Success = false, Message = "Your account is still under review." };
                }
            }

            return authResult;
        }
    }
}
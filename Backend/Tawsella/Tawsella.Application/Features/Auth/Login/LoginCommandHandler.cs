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
        private readonly IMapper _mapper;

        public LoginCommandHandler(
            IAuthService authService,
            IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken ct)
        {
            var loginDto = _mapper.Map<LoginDto>(request);

            return await _authService.LoginAsync(loginDto, ct);
        }
    }
}
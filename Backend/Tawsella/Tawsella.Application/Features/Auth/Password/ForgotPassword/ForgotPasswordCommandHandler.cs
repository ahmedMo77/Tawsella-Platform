using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Auth.Password.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, BaseToReturnDto>
    {
        private readonly IAuthService _authService;

        public ForgotPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<BaseToReturnDto> Handle(ForgotPasswordCommand request, CancellationToken ct)
        {
            return await _authService.ForgotPasswordAsync(request.Email, ct);
        }
    }
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs.AuthDTOS;
using MediatR;

namespace Tawsella.Application.Features.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResultDto>
    {
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<AuthResultDto> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            var result = await _tokenService.RefreshTokenAsync(request.RefreshToken);
            return result;
        }
    }
}
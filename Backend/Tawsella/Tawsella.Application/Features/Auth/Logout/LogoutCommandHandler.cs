using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Services;

namespace Tawsella.Application.Features.Auth.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly ITokenService _tokenService;

        public LogoutCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task Handle(LogoutCommand request, CancellationToken ct)
        {
            await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);
        }
    }
}

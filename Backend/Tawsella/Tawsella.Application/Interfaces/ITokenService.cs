using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;
using Tawsella.Domain.Entities;

namespace Tawsella.Application.Interfaces
{
    public interface ITokenService
    {
        Task<JwtResultDto> GenerateTokenAsync(AppUser user);
        RefreshToken CreateRefreshToken();
        Task<AuthResultDto> GenerateTokensPairAsync(AppUser user);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}

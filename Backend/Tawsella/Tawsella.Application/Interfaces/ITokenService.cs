using Tawsella.Application.DTOs.AuthDTOS;
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

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;
using Tawsella.Application.Interfaces;
using Tawsella.Application.Settings;
using Tawsella.Domain.Entities;

namespace Tawsella.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(UserManager<AppUser> userManager, IOptions<JwtSettings> jwtOptions)
        {
            _userManager = userManager;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<JwtResultDto> GenerateTokenAsync(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            // Claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(userClaims);


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();

            return new JwtResultDto
            {
                Token = tokenHandler.WriteToken(token),
                ExpireAt = token.ValidTo
            };
        }
        public RefreshToken CreateRefreshToken()
        {
            return new RefreshToken
            {
                Token = GenerateRandomToken(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30)
            };
        }
        public async Task<AuthResultDto> GenerateTokensPairAsync(AppUser user)
        {
            // تنظيف التوكنز منتهية الصلاحية (اختياري لكن يفضل عمله في Background Task)
            user.RefreshTokens.RemoveAll(t => !t.IsActive && t.CreatedAt.AddDays(30) < DateTime.UtcNow);

            var roles = await _userManager.GetRolesAsync(user);
            var jwt = await GenerateTokenAsync(user);
            var refreshToken = CreateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthResultDto
            {
                IsAuth = true,
                Successed = true,
                Message = "Login successful",
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList(),
                Token = jwt.Token,
                ExpireOn = jwt.ExpireAt,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpireOn = refreshToken.ExpiresAt
            };
        }
        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null) 
                return new AuthResultDto { Message = "Invalid refresh token" };

            var rt = user.RefreshTokens.First(t => t.Token == refreshToken);

            if (!rt.IsActive )
                return new AuthResultDto { Message = "Invalid refresh token" };


            rt.RevokedAt = DateTime.UtcNow;

            var newRt = CreateRefreshToken();
            user.RefreshTokens.Add(newRt);
            await _userManager.UpdateAsync(user);

            var jwt = await GenerateTokenAsync(user);

            return new AuthResultDto
            {
                IsAuth = true,
                Message = "Token refreshed successfully",

                UserName = user.UserName,
                Email = user.Email,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),

                Token = jwt.Token,
                ExpireOn = jwt.ExpireAt,

                RefreshToken = newRt.Token,
                RefreshTokenExpireOn = newRt.ExpiresAt
            };
        }
        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null) return;

            var rt = user.RefreshTokens.First(t => t.Token == refreshToken);
            if (!rt.IsActive) return;

            rt.RevokedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        private string GenerateRandomToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}

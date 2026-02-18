using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Tawsella.Application.Interfaces;

namespace Tawsella.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated or user ID claim is missing.");
            }

            return userId;
        }

        public string? GetUserIdOrNull()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string GetUserRole()
        {
            var role = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
            
            if (string.IsNullOrEmpty(role))
            {
                throw new UnauthorizedAccessException("User is not authenticated or role claim is missing.");
            }

            return role;
        }
    }
}

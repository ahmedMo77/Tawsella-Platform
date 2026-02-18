namespace Tawsella.Application.Interfaces
{
    /// <summary>
    /// Service for accessing the current authenticated user's information from JWT claims
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Gets the current authenticated user's ID from claims
        /// </summary>
        /// <returns>The user ID</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when user is not authenticated</exception>
        string GetUserId();

        /// <summary>
        /// Gets the current authenticated user's ID from claims, or null if not authenticated
        /// </summary>
        /// <returns>The user ID or null</returns>
        string? GetUserIdOrNull();

        /// <summary>
        /// Gets the current authenticated user's role from claims
        /// </summary>
        /// <returns>The user role</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when user is not authenticated</exception>
        string GetUserRole();
    }
}

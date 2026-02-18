using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tawsella.Domain.Entities;

namespace Tawsella.Application.Contracts
{
    public interface ICourierRepository : IAsyncRepository<Courier>
    {
        Task<Courier?> GetCourierWithProfileAsync(string courierId, CancellationToken cancellationToken = default);

        Task<Order?> GetActiveCourierOrderAsync(string courierId, CancellationToken cancellationToken = default);

        Task<(List<Order> orders, int totalCount)> GetAvailableOrdersAsync(string courierId, decimal latitude, decimal longitude, int page, int pageSize, CancellationToken cancellationToken = default);

        Task<(int totalDeliveries, decimal totalEarnings, double averageRating)> GetCourierDashboardStatsAsync(string courierId, CancellationToken cancellationToken = default);

        Task<List<Review>> GetCourierRecentReviewsAsync(string courierId, int count, CancellationToken cancellationToken = default);

        Task<Courier?> GetCourierPublicProfileAsync(string courierId, CancellationToken cancellationToken = default);

        Task<Courier?> GetCourierByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);
    }
}

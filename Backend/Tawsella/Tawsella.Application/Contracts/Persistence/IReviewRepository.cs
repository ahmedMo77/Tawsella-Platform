using System.Threading;
using System.Threading.Tasks;
using Tawsella.Application.Entities;

namespace Tawsella.Application.Contracts.Persistence
{
    public interface IReviewRepository : IAsyncRepository<Review>
    {
        Task<Review?> GetOrderReviewAsync(string orderId, string customerId, CancellationToken cancellationToken = default);

        Task AddReviewAndUpdateCourierStatsAsync(Review review, string courierId, int rating, CancellationToken cancellationToken = default);
    }
}

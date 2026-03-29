using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Domain.Entities;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Infrastructure.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Review?> GetOrderReviewAsync(
            string orderId,
            string customerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Where(r => r.OrderId == orderId && r.UserId == customerId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddReviewAndUpdateCourierStatsAsync(
            Review review,
            string courierId,
            int rating,
            CancellationToken cancellationToken = default)
        {
            // Add review
            await _context.Reviews.AddAsync(review, cancellationToken);

            // Update courier stats
            var courier = await _context.Couriers
                .FirstOrDefaultAsync(c => c.Id == courierId, cancellationToken);

            if (courier != null)
            {
                var totalReviews = await _context.Reviews
                    .CountAsync(r => r.CourierId == courierId, cancellationToken);

                var totalRating = await _context.Reviews
                    .Where(r => r.CourierId == courierId)
                    .SumAsync(r => r.Rating, cancellationToken);

                // Calculate new average including the new review
                courier.AverageRating = (totalRating + rating) / (double)(totalReviews + 1);
                courier.TotalReviews = totalReviews + 1;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

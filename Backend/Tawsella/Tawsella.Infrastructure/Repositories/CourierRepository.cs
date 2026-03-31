using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Infrastructure.Repositories
{
    public class CourierRepository : BaseRepository<Courier>, ICourierRepository
    {
        public CourierRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Courier?> GetCourierWithProfileAsync(string courierId, CancellationToken cancellationToken = default)
        {
            return await _context.Couriers
                .Include(c => c.User)
                .Include(c => c.Wallet)
                .FirstOrDefaultAsync(c => c.Id == courierId, cancellationToken);
        }

        public async Task<Courier?> GetCourierWithUserAsync(string id, CancellationToken ct)
        {
            return await _context.Couriers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<Order?> GetActiveCourierOrderAsync(string courierId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Courier)
                .Where(o => o.CourierId == courierId &&
                           (o.Status == OrderStatus.Accepted ||
                            o.Status == OrderStatus.PickedUp))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<(List<Order> orders, int totalCount)> GetAvailableOrdersAsync(
            string courierId,
            decimal latitude,
            decimal longitude,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            // Get courier to check if they have active order
            var courier = await _context.Couriers
                .FirstOrDefaultAsync(c => c.Id == courierId, cancellationToken);

            if (courier == null)
                return (new List<Order>(), 0);

            // Available orders: Pending status, not assigned to any courier
            var query = _context.Orders
                .Include(o => o.Courier)
                .Where(o => o.Status == OrderStatus.Pending && o.CourierId == null);

            var totalCount = await query.CountAsync(cancellationToken);

            var orders = await query
                .OrderBy(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (orders, totalCount);
        }

        public async Task<(int totalDeliveries, decimal totalEarnings, double averageRating)> GetCourierDashboardStatsAsync(
            string courierId,
            CancellationToken cancellationToken = default)
        {
            var courier = await _context.Couriers
                .FirstOrDefaultAsync(c => c.Id == courierId, cancellationToken);

            if (courier == null)
                return (0, 0, 0);

            var totalDeliveries = await _context.Orders
                .CountAsync(o => o.CourierId == courierId && o.Status == OrderStatus.Delivered, cancellationToken);

            var totalEarnings = await _context.Orders
                .Where(o => o.CourierId == courierId && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.Money.CourierEarnings ?? 0, cancellationToken);

            return (totalDeliveries, totalEarnings, courier.AverageRating);
        }

        public async Task<List<Review>> GetCourierRecentReviewsAsync(
            string courierId,
            int count,
            CancellationToken cancellationToken = default)
        {
            return await _context.Reviews
                .Where(r => r.CourierId == courierId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<Courier?> GetCourierPublicProfileAsync(string courierId, CancellationToken cancellationToken = default)
        {
            return await _context.Couriers
                .FirstOrDefaultAsync(c => c.Id == courierId, cancellationToken);
        }

        public async Task<Courier?> GetCourierByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
        {
            return await _context.Couriers
                .FirstOrDefaultAsync(c => c.NationalId == nationalId, cancellationToken);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Infrastructure.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetCustomerProfileAsync(string customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);
        }

        public async Task<(int totalOrders, decimal totalSpent)> GetCustomerStatisticsAsync(
            string customerId,
            CancellationToken cancellationToken = default)
        {
            var totalOrders = await _context.Orders
                .CountAsync(o => o.CustomerId == customerId, cancellationToken);

            var totalSpent = await _context.Orders
                .Where(o => o.CustomerId == customerId && o.Status == OrderStatus.Delivered)
                .SumAsync(o => o.Money.FinalPrice ?? o.Money.EstimatedPrice, cancellationToken);

            return (totalOrders, totalSpent);
        }
    }
}

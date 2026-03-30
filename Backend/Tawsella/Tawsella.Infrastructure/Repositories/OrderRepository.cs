using Microsoft.EntityFrameworkCore;
using System;
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
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Order?> GetOrderWithDetailsAsync(string orderId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Courier)
                .Include(o => o.Customer)
                .Where(o => o.Id == orderId && (o.CustomerId == userId || o.CourierId == userId))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddStatusHistoryAsync(string orderId, OrderStatus status, string notes)
        {
            await _context.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = orderId,
                Status = status,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<(List<Order> orders, int totalCount)> GetOrdersHistoryAsync(
            string userId,
            OrderStatus? status,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Orders
                .Where(o => o.CustomerId == userId || o.CourierId == userId);

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var orders = await query
                .Include(o => o.Courier)
                .Include(o => o.Customer)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (orders, totalCount);
        }

        public async Task<List<OrderApplication>> GetOrderApplicationsAsync(
            string orderId,
            string customerId,
            CancellationToken cancellationToken = default)
        {
            return await _context.OrderApplications
                .Include(oa => oa.Courier)
                .Where(oa => oa.OrderId == orderId && oa.Order.CustomerId == customerId)
                .OrderBy(oa => oa.AppliedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetOrderForCourierAsync(string orderId, string courierId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Courier)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CourierId == courierId, cancellationToken);
        }

        public async Task<Order?> GetOrderForCustomerAsync(string orderId, string customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Courier)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId, cancellationToken);
        }

        public async Task<bool> CustomerExistsAsync(string customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Customers.AnyAsync(c => c.Id == customerId, cancellationToken);
        }

        public async Task<bool> HasCourierAppliedAsync(string orderId, string courierId, CancellationToken cancellationToken = default)
        {
            return await _context.OrderApplications
                .AnyAsync(a => a.OrderId == orderId && a.CourierId == courierId, cancellationToken);
        }

        public async Task AddOrderApplicationAsync(OrderApplication application, CancellationToken cancellationToken = default)
        {
            await _context.OrderApplications.AddAsync(application, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<OrderApplication?> GetOrderApplicationWithCourierAsync(string applicationId, string orderId, CancellationToken cancellationToken = default)
        {
            return await _context.OrderApplications
                .Include(a => a.Courier)
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.OrderId == orderId, cancellationToken);
        }

        public async Task ApproveApplicationAsync(Order order, OrderApplication approvedApp, string orderId, string applicationId, CancellationToken cancellationToken = default)
        {
            approvedApp.Status = OrderApplicationStatus.Approved;
            order.CourierId = approvedApp.CourierId;
            order.Status = OrderStatus.Accepted;
            order.AcceptedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            approvedApp.Courier.IsAvailable = false;

            await _context.OrderApplications
                .Where(a => a.OrderId == orderId && a.Id != applicationId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.Status, OrderApplicationStatus.Rejected)
                    .SetProperty(a => a.RejectedReason, "Another courier selected"), cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Order?> GetOrderWithCourierAsync(string orderId, string customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Courier)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId, cancellationToken);
        }

        public async Task<Order?> GetOrderForDeliveryAsync(string orderId, string courierId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Include(o => o.Courier).ThenInclude(c => c.Wallet)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CourierId == courierId, cancellationToken);
        }

        public async Task CompleteDeliveryAsync(Order order, WalletTransaction? transaction, CancellationToken cancellationToken = default)
        {
            if (transaction != null)
            {
                await _context.WalletTransactions.AddAsync(transaction, cancellationToken);
            }

            await _context.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = OrderStatus.Delivered,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

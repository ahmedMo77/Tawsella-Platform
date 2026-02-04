using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Interfaces;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Admins = new BaseRepository<Admin>(context);
            Customers = new BaseRepository<Customer>(context);
            Couriers = new BaseRepository<Courier>(context);
            Orders = new BaseRepository<Order>(context);
            OrderStatusHistories = new BaseRepository<OrderStatusHistory>(context);
            Reviews = new BaseRepository<Review>(context);
            Notifications = new BaseRepository<Notification>(context);
            OrderApplications = new BaseRepository<OrderApplication>(context);
            Wallets = new BaseRepository<Wallet>(context);
            WalletTransactions = new BaseRepository<WalletTransaction>(context);
        }

        public IBaseRepository<Admin> Admins { get; }
        public IBaseRepository<Customer> Customers { get; }
        public IBaseRepository<Courier> Couriers { get; }
        public IBaseRepository<Order> Orders { get; }
        public IBaseRepository<OrderStatusHistory> OrderStatusHistories { get; }
        public IBaseRepository<Review> Reviews { get; }
        public IBaseRepository<Notification> Notifications { get; }
        public IBaseRepository<OrderApplication> OrderApplications { get; }
        public IBaseRepository<Wallet> Wallets { get; }
        public IBaseRepository<WalletTransaction> WalletTransactions { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

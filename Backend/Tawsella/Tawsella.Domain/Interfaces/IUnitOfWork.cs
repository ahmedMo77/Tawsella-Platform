using Tawsella.Domain.Entities;

namespace Tawsella.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work: groups one or more repository operations into a single transaction.
    /// Call SaveChangesAsync() to persist all changes.
    /// </summary>
    public interface IUnitOfWork
    {
        IBaseRepository<Admin> Admins { get; }
        IBaseRepository<Customer> Customers { get; }
        IBaseRepository<Courier> Couriers { get; }
        IBaseRepository<Order> Orders { get; }
        IBaseRepository<OrderStatusHistory> OrderStatusHistories { get; }
        IBaseRepository<Review> Reviews { get; }
        IBaseRepository<Notification> Notifications { get; }
        IBaseRepository<OrderApplication> OrderApplications { get; }
        IBaseRepository<Wallet> Wallets { get; }
        IBaseRepository<WalletTransaction> WalletTransactions { get; }

        System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default);
    }
}

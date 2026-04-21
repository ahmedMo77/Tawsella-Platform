using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;
using Tawsella.Domain.Entities;

namespace Tawsella.Application.Contracts.Persistence
{
    /// <summary>
    /// Repository for Order-related operations.
    /// </summary>
    public interface IOrderRepository : IAsyncRepository<Order>
    {
        Task AddStatusHistoryAsync(string orderId, OrderStatus status, string notes, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get order with full details for a specific user (customer or courier).
        /// Used by: GetOrderDetailsQuery
        /// </summary>
        Task<Order?> GetOrderWithDetailsAsync(string orderId, string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get paginated orders history for a user (as customer or courier).
        /// Used by: GetOrdersHistoryQuery
        /// </summary>
        Task<(List<Order> orders, int totalCount)> GetOrdersHistoryAsync(
            string userId, 
            OrderStatus? status, 
            int page, 
            int pageSize, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get order applications for a specific order (customer only).
        /// Used by: GetOrderApplicationsQuery
        /// </summary>
        Task<List<OrderApplication>> GetOrderApplicationsAsync(
            string orderId, 
            string customerId, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get order for courier operations (pickup, deliver, update status).
        /// Used by: PickupOrderCommand, DeliverOrderCommand, UpdateOrderStatusCommand
        /// </summary>
        Task<Order?> GetOrderForCourierAsync(string orderId, string courierId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get order for customer operations (cancel, approve application).
        /// Used by: CancelOrderCommand, ApproveOrderApplicationCommand
        /// </summary>
        Task<Order?> GetOrderForCustomerAsync(string orderId, string customerId, CancellationToken cancellationToken = default);



        /// <summary>
        /// Check if a courier has already applied for an order.
        /// Used by: ApplyForOrderCommand
        /// </summary>
        Task<bool> HasCourierAppliedAsync(string orderId, string courierId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add an order application.
        /// Used by: ApplyForOrderCommand
        /// </summary>
        Task AddOrderApplicationAsync(OrderApplication application, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get order application with courier info for approval.
        /// Used by: ApproveOrderApplicationCommand
        /// </summary>
        Task<OrderApplication?> GetOrderApplicationWithCourierAsync(string applicationId, string orderId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Approve an application: set order courier, reject all other applications, save.
        /// Used by: ApproveOrderApplicationCommand
        /// </summary>
        Task ApproveApplicationAsync(Order order, OrderApplication approvedApp, string orderId, string applicationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cancel an order and optionally free the courier.
        /// Used by: CancelOrderCommand
        /// </summary>
        Task<Order?> GetOrderWithCourierAsync(string orderId, string customerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get order for courier with wallet info for delivery.
        /// Used by: DeliverOrderCommand
        /// </summary>
        Task<Order?> GetOrderForDeliveryAsync(string orderId, string courierId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Complete delivery: update order, wallet, add transaction and status history.
        /// Used by: DeliverOrderCommand
        /// </summary>
        Task CompleteDeliveryAsync(Order order, WalletTransaction? transaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// Save changes to the repository context.
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        Task UpdatePaymentStatusAsync(string orderId, PaymentStatus paymentStatus, string notes, CancellationToken cancellationToken = default);
    }
}

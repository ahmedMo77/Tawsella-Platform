using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tawsella.Application.Entities;

namespace Tawsella.Application.Contracts.Persistence
{
    public interface INotificationRepository : IAsyncRepository<Notification>
    {
        Task<(List<Notification> notifications, int totalCount)> GetUserNotificationsAsync(string userId, bool unreadOnly, int page, int pageSize, CancellationToken cancellationToken = default);

        Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default);

        Task<Notification?> MarkNotificationAsReadAsync(string notificationId, string userId, CancellationToken cancellationToken = default);
    }
}

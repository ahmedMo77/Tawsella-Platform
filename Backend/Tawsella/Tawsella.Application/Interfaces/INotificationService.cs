using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.DTOs.NotificationDTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string userId, string title, string message, NotificationType type);

        Task<PaginatedResponseDto<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly, int page, int pageSize);

        Task<bool> MarkAsReadAsync(string userId, string notificationId);

        Task MarkAllAsReadAsync(string userId);

        // ممكن تزيد مستقبلاً لو محتاج عداد للإشعارات غير المقروءة
        // Task<int> GetUnreadCountAsync(string userId);
    }
}

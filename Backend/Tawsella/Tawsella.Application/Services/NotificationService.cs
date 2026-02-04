using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.DTOs.NotificationDTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Domain.Interfaces;

namespace Tawsella.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task SendNotificationAsync(string userId, string title, string message, NotificationType type)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PaginatedResponseDto<NotificationDto>> GetUserNotificationsAsync(string userId, bool unreadOnly, int page, int pageSize)
        {
            var query = _unitOfWork.Notifications.Query
                .Where(n => n.UserId == userId)
                .AsNoTracking();

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            var totalCount = await query.CountAsync();
            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PaginatedResponseDto<NotificationDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = notifications,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<bool> MarkAsReadAsync(string userId, string notificationId)
        {
            var notification = await _unitOfWork.Notifications.Query
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null || notification.IsRead) return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            await _unitOfWork.Notifications.Query
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(n => n.IsRead, true)
                    .SetProperty(n => n.ReadAt, DateTime.UtcNow));
        }
    }
}

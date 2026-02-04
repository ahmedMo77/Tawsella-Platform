using System;
using Tawsella.Domain.Enums;

namespace Tawsella.Domain.DTOs.NotificationDTOs
{
    public class NotificationDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}

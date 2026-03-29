using MediatR;
using Tawsella.Application.Enums;

namespace Tawsella.Application.Features.Notifications.Commands.SendNotification
{
    public class SendNotificationCommand : IRequest
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
    }
}

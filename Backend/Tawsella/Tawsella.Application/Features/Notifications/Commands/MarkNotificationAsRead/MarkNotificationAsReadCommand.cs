using MediatR;

namespace Tawsella.Application.Features.Notifications.Commands.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommand : IRequest<MarkNotificationAsReadCommandResponse>
    {
        public string NotificationId { get; set; }
    }
}

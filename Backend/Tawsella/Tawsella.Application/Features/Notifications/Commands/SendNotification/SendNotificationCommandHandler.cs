using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Domain.Entities;

namespace Tawsella.Application.Features.Notifications.Commands.SendNotification
{
    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand>
    {
        private readonly INotificationRepository _notificationRepository;

        public SendNotificationCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                Title = request.Title,
                Message = request.Message,
                Type = request.Type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            await _notificationRepository.AddAsync(notification, cancellationToken);
        }
    }
}

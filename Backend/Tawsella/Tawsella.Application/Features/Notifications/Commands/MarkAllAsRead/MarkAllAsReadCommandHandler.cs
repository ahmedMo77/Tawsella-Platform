using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;

namespace Tawsella.Application.Features.Notifications.Commands.MarkAllAsRead
{
    public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;

        public MarkAllAsReadCommandHandler(
            INotificationRepository notificationRepository,
            ICurrentUserService currentUserService)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            
            await _notificationRepository.MarkAllAsReadAsync(userId, cancellationToken);
        }
    }
}

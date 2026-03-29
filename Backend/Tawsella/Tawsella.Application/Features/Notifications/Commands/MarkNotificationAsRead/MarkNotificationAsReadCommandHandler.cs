using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;

namespace Tawsella.Application.Features.Notifications.Commands.MarkNotificationAsRead
{
    public class MarkNotificationAsReadCommandHandler 
        : IRequestHandler<MarkNotificationAsReadCommand, MarkNotificationAsReadCommandResponse>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ICurrentUserService _currentUserService;

        public MarkNotificationAsReadCommandHandler(
            INotificationRepository notificationRepository,
            ICurrentUserService currentUserService)
        {
            _notificationRepository = notificationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<MarkNotificationAsReadCommandResponse> Handle(
            MarkNotificationAsReadCommand request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            
            var notification = await _notificationRepository.MarkNotificationAsReadAsync(
                request.NotificationId, 
                userId, 
                cancellationToken);

            if (notification == null)
            {
                return new MarkNotificationAsReadCommandResponse
                {
                    Success = false
                };
            }

            return new MarkNotificationAsReadCommandResponse
            {
                Success = true
            };
        }
    }
}

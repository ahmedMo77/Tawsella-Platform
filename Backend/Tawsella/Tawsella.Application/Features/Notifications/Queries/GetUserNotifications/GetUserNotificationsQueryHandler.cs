using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs.NotificationDTOs;

namespace Tawsella.Application.Features.Notifications.Queries.GetUserNotifications
{
    public class GetUserNotificationsQueryHandler 
        : IRequestHandler<GetUserNotificationsQuery, GetUserNotificationsQueryResponse>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetUserNotificationsQueryHandler(
            INotificationRepository notificationRepository, 
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GetUserNotificationsQueryResponse> Handle(
            GetUserNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            
            var (notifications, totalCount) = await _notificationRepository.GetUserNotificationsAsync(
                userId,
                request.UnreadOnly,
                request.Page,
                request.PageSize,
                cancellationToken);

            var notificationDtos = _mapper.Map<List<NotificationDto>>(notifications);

            return new GetUserNotificationsQueryResponse
            {
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                Items = notificationDtos,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }
}

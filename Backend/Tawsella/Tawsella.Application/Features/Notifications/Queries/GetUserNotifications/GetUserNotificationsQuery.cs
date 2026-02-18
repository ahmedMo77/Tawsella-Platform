using MediatR;

namespace Tawsella.Application.Features.Notifications.Queries.GetUserNotifications
{
    public class GetUserNotificationsQuery : IRequest<GetUserNotificationsQueryResponse>
    {
        public bool UnreadOnly { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

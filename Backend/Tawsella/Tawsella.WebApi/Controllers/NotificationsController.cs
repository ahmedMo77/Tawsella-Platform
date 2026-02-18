using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawsella.Application.Features.Notifications.Commands.MarkAllAsRead;
using Tawsella.Application.Features.Notifications.Commands.MarkNotificationAsRead;
using Tawsella.Application.Features.Notifications.Queries.GetUserNotifications;

namespace Tawsella.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetUserNotificationsQuery
            {
                UnreadOnly = unreadOnly,
                Page = page,
                PageSize = pageSize
            });
            return Ok(result);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            var result = await _mediator.Send(new MarkNotificationAsReadCommand { NotificationId = id });
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _mediator.Send(new MarkAllAsReadCommand());
            return Ok(new { Message = "All notifications marked as read" });
        }
    }
}

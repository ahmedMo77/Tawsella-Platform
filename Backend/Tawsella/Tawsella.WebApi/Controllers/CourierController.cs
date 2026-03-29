using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawsella.Application.Features.Couriers.Commands.ApplyForOrder;
using Tawsella.Application.Features.Couriers.Commands.DeliverOrder;
using Tawsella.Application.Features.Couriers.Commands.PickupOrder;
using Tawsella.Application.Features.Couriers.Commands.UpdateCourierProfile;
using Tawsella.Application.Features.Couriers.Commands.UpdateLocation;
using Tawsella.Application.Features.Couriers.Commands.UpdateOnlineStatus;
using Tawsella.Application.Features.Couriers.Queries.GetActiveOrder;
using Tawsella.Application.Features.Couriers.Queries.GetAvailableOrders;
using Tawsella.Application.Features.Couriers.Queries.GetCourierProfile;

namespace Tawsella.WebApi.Controllers
{
    [Authorize(Roles = "Courier")]
    [ApiController]
    [Route("api/[controller]")]
    public class CourierController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CourierController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _mediator.Send(new GetCourierProfileQuery());
            if (result == null) return NotFound("Courier not found.");
            return Ok(result);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateCourierProfileCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] bool isOnline)
        {
            var result = await _mediator.Send(new UpdateOnlineStatusCommand { IsOnline = isOnline });
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update-location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { Message = "Location updated" });
        }

        [HttpGet("available-orders")]
        public async Task<IActionResult> GetAvailableOrders([FromQuery] double radius = 10)
        {
            var result = await _mediator.Send(new GetAvailableOrdersQuery(radius));
            return Ok(result);
        }

        [HttpPost("apply-order")]
        public async Task<IActionResult> ApplyForOrder([FromBody] ApplyForOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("pickup-order")]
        public async Task<IActionResult> PickupOrder([FromBody] PickupOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("deliver-order")]
        public async Task<IActionResult> DeliverOrder([FromBody] DeliverOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active-order")]
        public async Task<IActionResult> GetActiveOrder()
        {
            var result = await _mediator.Send(new GetActiveOrderQuery());
            if (result == null) return NotFound("No active orders found.");
            return Ok(result);
        }
    }
}

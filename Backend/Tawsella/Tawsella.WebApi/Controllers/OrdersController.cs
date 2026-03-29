//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Tawsella.Application.Features.Orders.Commands.ApproveOrderApplication;
//using Tawsella.Application.Features.Orders.Commands.CancelOrder;
//using Tawsella.Application.Features.Orders.Commands.CreateOrder;
//using Tawsella.Application.Features.Orders.Commands.UpdateOrderStatus;
//using Tawsella.Application.Features.Orders.Queries.GetOrderApplications;
//using Tawsella.Application.Features.Orders.Queries.GetOrderDetails;
//using Tawsella.Application.Features.Orders.Queries.GetOrdersHistory;
//using Tawsella.Application.Features.Reviews.Commands.SubmitReview;
//using Tawsella.Application.DTOs.ReviewDTOs;
//using Tawsella.Domain.Enums;

//namespace Tawsella.WebApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize(Roles = "Customer")]
//    public class OrdersController : ControllerBase
//    {
//        private readonly IMediator _mediator;

//        public OrdersController(IMediator mediator)
//        {
//            _mediator = mediator;
//        }

//        [HttpPost("create")]
//        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
//        {
//            var result = await _mediator.Send(command);
//            return result.Success ? Ok(result) : BadRequest(result);
//        }

//        [HttpGet("{id}")]
//        [Authorize(Roles = "Customer,Courier")]
//        public async Task<IActionResult> GetOrderDetails(string id)
//        {
//            var result = await _mediator.Send(new GetOrderDetailsQuery { orderId = id });
//            if (result == null) return NotFound(new { Message = "Order not found or you don't have permission." });
//            return Ok(result);
//        }

//        [HttpGet("history")]
//        [Authorize(Roles = "Customer,Courier")]
//        public async Task<IActionResult> GetHistory(
//            [FromQuery] OrderStatus? status,
//            [FromQuery] int page = 1,
//            [FromQuery] int pageSize = 10)
//        {
//            var result = await _mediator.Send(new GetOrdersHistoryQuery
//            {
//                status = status,
//                page = page,
//                pageSize = pageSize
//            });
//            return Ok(result);
//        }

//        [HttpPost("{id}/cancel")]
//        public async Task<IActionResult> CancelOrder(string id, [FromBody] string reason)
//        {
//            var result = await _mediator.Send(new CancelOrderCommand { orderId = id, reason = reason });
//            return result.Success ? Ok(result) : BadRequest(result);
//        }

//        [HttpGet("{id}/applications")]
//        public async Task<IActionResult> GetApplications(string id)
//        {
//            var result = await _mediator.Send(new GetOrderApplicationsQuery { orderId = id });
//            if (result == null) return NotFound(new { Message = "Order not found." });
//            return Ok(result);
//        }

//        [HttpPost("{id}/approve/{applicationId}")]
//        public async Task<IActionResult> ApproveApplication(string id, string applicationId)
//        {
//            var result = await _mediator.Send(new ApproveOrderApplicationCommand
//            {
//                orderId = id,
//                applicationId = applicationId
//            });
//            return result.Success ? Ok(result) : BadRequest(result);
//        }

//        [HttpPost("{id}/review")]
//        public async Task<IActionResult> SubmitReview(string id, [FromBody] CreateReviewDto dto)
//        {
//            var result = await _mediator.Send(new SubmitReviewCommand { orderId = id, dto = dto });
//            return result.Success ? Ok(result) : BadRequest(result);
//        }

//        [HttpPatch("{id}/status")]
//        [Authorize(Roles = "Courier")]
//        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] OrderStatus status, [FromQuery] string? notes)
//        {
//            var result = await _mediator.Send(new UpdateOrderStatusCommand
//            {
//                OrderId = id,
//                NewStatus = status,
//                Notes = notes
//            });
//            return result.Success ? Ok(result) : BadRequest(result);
//        }
//    }
//}

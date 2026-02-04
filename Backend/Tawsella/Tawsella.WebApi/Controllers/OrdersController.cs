using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs.OrderDTOs;
using Tawsella.Domain.DTOs.ReviewDTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            try
            {
                var result = await _orderService.CreateOrderAsync(CurrentUserId, dto);
                return result.Success ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderDetails(string id)
        {
            try
            {
                var order = await _orderService.GetOrderDetailsAsync(id, CurrentUserId);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "Order not found or you don't have permission." });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory([FromQuery] OrderStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetOrdersHistoryAsync(CurrentUserId, status, page, pageSize);
            return Ok(result);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(string id, [FromBody] string reason)
        {
            var result = await _orderService.CancelOrderAsync(CurrentUserId, id, reason);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id}/applications")]
        public async Task<IActionResult> GetApplications(string id)
        {
            try
            {
                var apps = await _orderService.GetOrderApplicationsAsync(CurrentUserId, id);
                return Ok(apps);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("{id}/approve/{applicationId}")]
        public async Task<IActionResult> ApproveApplication(string id, string applicationId)
        {
            var result = await _orderService.ApproveOrderApplicationAsync(CurrentUserId, id, applicationId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id}/review")]
        public async Task<IActionResult> SubmitReview(string id, [FromBody] CreateReviewDto dto)
        {
            var result = await _orderService.SubmitReviewAsync(CurrentUserId, id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Courier")]
        public async Task<IActionResult> UpdateStatus(string id, [FromQuery] OrderStatus status, [FromQuery] string? notes)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, CurrentUserId, status, notes);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

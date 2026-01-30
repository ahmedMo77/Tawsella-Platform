using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.CustomerDTOs;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.Enums;

namespace Tawsella.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        private string GetCustomerId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Get customer profile
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var customerId = GetCustomerId();
            var profile = await _customerService.GetProfile(customerId);

            if (profile == null)
                return NotFound("Customer profile not found");

            return Ok(profile);
        }

        /// <summary>
        /// Update customer profile
        /// </summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerId = GetCustomerId();
            var result = await _customerService.UpdateCustomerProfile(customerId, dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        /// <summary>
        /// Get customer statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var customerId = GetCustomerId();
            var statistics = await _customerService.GetCustomerStatistics(customerId);

            return Ok(statistics);
        }

        /// <summary>
        /// Calculate order price estimate
        /// </summary>
        [HttpPost("orders/calculate-price")]
        public async Task<IActionResult> CalculatePrice([FromBody] CalculatePriceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var estimate = await _customerService.CalculateOrderPrice(dto);

            return Ok(estimate);
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerId = GetCustomerId();
            var result = await _customerService.CreateOrder(customerId, dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        /// <summary>
        /// Get customer orders with optional status filter and pagination
        /// </summary>
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(
            [FromQuery] OrderStatus? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var customerId = GetCustomerId();
            var orders = await _customerService.GetCustomerOrders(customerId, status, page, pageSize);

            return Ok(orders);
        }

        /// <summary>
        /// Get specific order details
        /// </summary>
        [HttpGet("orders/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(string orderId)
        {
            var customerId = GetCustomerId();
            var order = await _customerService.GetOrderDetails(customerId, orderId);

            if (order == null)
                return NotFound("Order not found");

            return Ok(order);
        }

        /// <summary>
        /// Cancel an order
        /// </summary>
        [HttpPost("orders/{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(string orderId, [FromBody] string reason)
        {
            var customerId = GetCustomerId();
            var result = await _customerService.CancelOrder(customerId, orderId, reason);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        /// <summary>
        /// Submit a review for a delivered order
        /// </summary>
        [HttpPost("orders/{orderId}/review")]
        public async Task<IActionResult> SubmitReview(string orderId, [FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerId = GetCustomerId();
            var result = await _customerService.SubmitReview(customerId, orderId, dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        /// <summary>
        /// Get review for a specific order
        /// </summary>
        [HttpGet("orders/{orderId}/review")]
        public async Task<IActionResult> GetOrderReview(string orderId)
        {
            var customerId = GetCustomerId();
            var review = await _customerService.GetOrderReview(customerId, orderId);

            if (review == null)
                return NotFound("Review not found");

            return Ok(review);
        }

        /// <summary>
        /// Get customer notifications with pagination
        /// </summary>
        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            var customerId = GetCustomerId();
            var notifications = await _customerService.GetCustomerNotifications(customerId, unreadOnly, page, pageSize);

            return Ok(notifications);
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [HttpPut("notifications/{notificationId}/read")]
        public async Task<IActionResult> MarkNotificationAsRead(string notificationId)
        {
            var customerId = GetCustomerId();
            var result = await _customerService.MarkNotificationAsRead(customerId, notificationId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPut("notifications/read-all")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var customerId = GetCustomerId();
            var result = await _customerService.MarkAllNotificationsAsRead(customerId);

            return Ok(result);
        }
    }
}

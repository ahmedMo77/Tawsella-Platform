using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs.CourierDTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.WebApi.Controllers
{
    [Authorize(Roles = "Courier")]
    [ApiController]
    [Route("api/[controller]")]
    public class CourierController : ControllerBase
    {
        private readonly ICourierService _courierService;

        public CourierController(ICourierService courierService)
        {
            _courierService = courierService;
        }

        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(string id)
        {
            var profile = await _courierService.GetProfileAsync(id);
            if (profile == null) return NotFound("Courier not found.");
            return Ok(profile);
        }

        [HttpPut("update-profile/{id}")]
        public async Task<IActionResult> UpdateProfile(string id, UpdateCourierProfileDto model)
        {
            var result = await _courierService.UpdateProfileAsync(id, model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("status/{id}")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] bool isOnline)
        {
            var result = await _courierService.UpdateOnlineStatusAsync(id, isOnline);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update-location/{id}")]
        public async Task<IActionResult> UpdateLocation(string id, UpdateLocationDto location)
        {
            await _courierService.UpdateLocationAsync(id, location);
            return Ok(new { message = "Location updated" });
        }

        [HttpGet("available-orders/{id}")]
        public async Task<IActionResult> GetAvailableOrders(string id, [FromQuery] double radius = 10)
        {
            var orders = await _courierService.GetAvailableOrdersAsync(id, radius);
            return Ok(orders);
        }

        [HttpPost("apply-order")]
        public async Task<IActionResult> ApplyForOrder(string courierId, string orderId)
        {
            var result = await _courierService.ApplyForOrderAsync(courierId, orderId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("pickup-order")]
        public async Task<IActionResult> PickupOrder(string courierId, string orderId)
        {
            var result = await _courierService.PickupOrderAsync(courierId, orderId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("deliver-order")]
        public async Task<IActionResult> DeliverOrder(string courierId, string orderId)
        {
            var result = await _courierService.DeliverOrderAsync(courierId, orderId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("active-order/{id}")]
        public async Task<IActionResult> GetActiveOrder(string id)
        {
            var order = await _courierService.GetActiveOrderAsync(id);
            if (order == null) return NotFound("No active orders found for this courier.");
            return Ok(order);
        }
    }
}

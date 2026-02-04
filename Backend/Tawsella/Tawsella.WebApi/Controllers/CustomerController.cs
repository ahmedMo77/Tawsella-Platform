using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
        private readonly INotificationService _notificationService;

        public CustomerController(ICustomerService customerService, INotificationService notificationService)
        {
            _customerService = customerService;
            _notificationService = notificationService;
        }

        private string CurrentCustomerId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;


        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var profile = await _customerService.GetProfile(CurrentCustomerId);
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _customerService.UpdateCustomerProfile(CurrentCustomerId, dto);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var stats = await _customerService.GetCustomerStatistics(CurrentCustomerId);
                return Ok(stats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        // Get courier public profile (rating, reviews, etc) when viewing an applicant.   // maybe move to CourierController later
        [HttpGet("courier-profile/{courierId}")]
        public async Task<IActionResult> GetCourierProfile(string courierId)
        {
            try
            {
                var profile = await _customerService.GetCourierPublicProfileAsync(courierId);
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}

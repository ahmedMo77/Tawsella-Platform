using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawsella.Application.Features.Customers.Commands.UpdateCustomerProfile;
using Tawsella.Application.Features.Customers.Queries.GetCourierPublicProfile;
using Tawsella.Application.Features.Customers.Queries.GetCustomerProfile;
using Tawsella.Application.Features.Customers.Queries.GetCustomerStatistics;

namespace Tawsella.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _mediator.Send(new GetCustomerProfileQuery());
            if (result == null) return NotFound(new { Message = "Customer not found." });
            return Ok(result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerProfileCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _mediator.Send(new GetCustomerStatisticsQuery());
            if (result == null) return NotFound(new { Message = "Customer not found." });
            return Ok(result);
        }

        [HttpGet("courier-profile/{courierId}")]
        public async Task<IActionResult> GetCourierProfile(string courierId)
        {
            var result = await _mediator.Send(new GetCourierPublicProfileQuery { CourierId = courierId });
            if (result == null) return NotFound(new { Message = "Courier not found." });
            return Ok(result);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawsella.Application.Features.Admin.Commands.CreateAdmin;
using Tawsella.Application.Features.Couriers.Commands.ApproveCourier;

namespace Tawsella.WebApi.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-admin")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("approve-courier/{courierId}")]
        public async Task<IActionResult> ApproveCourier(string courierId)
        {
            var result = await _mediator.Send(new ApproveCourierCommand(courierId));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

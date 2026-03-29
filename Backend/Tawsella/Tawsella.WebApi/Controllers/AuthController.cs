using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Application.Features.Auth.ConfirmEmail;
using Tawsella.Application.Features.Auth.Login;
using Tawsella.Application.Features.Auth.Logout;
using Tawsella.Application.Features.Auth.Password.ChangePassword;
using Tawsella.Application.Features.Auth.Password.ForgotPassword;
using Tawsella.Application.Features.Auth.Password.ResetPassword;
using Tawsella.Application.Features.Auth.RefreshToken;
using Tawsella.Application.Features.Auth.Register.RegisterCourier;
using Tawsella.Application.Features.Auth.Register.RegisterCustomer;

namespace Tawsella.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("register/courier")]
        public async Task<IActionResult> RegisterCourier([FromBody] RegisterCourierCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsAuth ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _mediator.Send(new RefreshTokenCommand(dto.RefreshToken));
            return result.IsAuth ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
        {
            await _mediator.Send(new LogoutCommand(dto.RefreshToken));
            return NoContent();
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result); // always 200 to prevent email enumeration
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}

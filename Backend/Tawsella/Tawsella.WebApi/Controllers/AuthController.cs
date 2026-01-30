using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Tls;
using System.Security.Claims;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Application.Interfaces;

namespace Tawsella.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("admin")]
        public async Task<IActionResult> CreateAdmin(CreateAdminDto dto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.CreateAdminAsync(dto);

            if(!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("customer")]
        public async Task<IActionResult> RegisterCustomer(RegisterUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _authService.RegisterCustomerAsync(dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("courier")]
        public async Task<IActionResult> RegisterCourier(RegisterCourierDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _authService.RegisterCourierAsync(dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("merchant")]
        public async Task<IActionResult> RegisterMerchant(RegisterMerchantDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.RegisterMerchantAsync(dto);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);

            if(!result.Successed) 
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string token)
        {
            await _authService.LogoutAsync(token);
            return NoContent();
        }


        [HttpPost("aprove-courier")]
        public async Task<IActionResult> AproveCourier(string courierId)
        {
            var result = await _authService.ApproveCourierAsync(courierId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("aprove-merchant")]
        public async Task<IActionResult> AproveMerchant(string merchantId)
        {
            var result = await _authService.ApproveMerchantAsync(merchantId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string code)
        {
            var dto = new ConfirmEmailDto
            {
                email = email,
                code = code
            };

            var result = await _authService.ConfirmEmailAsync(dto);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _authService.ForgotPasswordAsync(email);

            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(dto);

            if (!result.Success) 
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var dto = new ChangePasswordDto
            {
                userId = userId,
                oldPassword = oldPassword,
                newPassword = newPassword
            };

            var result = await _authService.ChangePasswordAsync(dto);

            if (!result.Success) 
                return BadRequest(result);

            return Ok(result);
            
        }
    }
}

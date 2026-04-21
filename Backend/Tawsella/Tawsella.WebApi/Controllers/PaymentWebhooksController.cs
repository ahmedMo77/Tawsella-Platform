using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs.PaymentDTOs;
using Tawsella.Application.Features.Orders.Commands.ProcessPaymentWebhook;
using Tawsella.Infrastructure.Models;

namespace Tawsella.WebApi.Controllers
{
    [Route("api/webhooks")]
    [ApiController]
    public class PaymentWebhooksController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IPaymentService _paymentService;
        private readonly FawaterkSettings _settings;
        private readonly ILogger<PaymentWebhooksController> _logger;

        public PaymentWebhooksController(
            ISender sender,
            IPaymentService paymentService,
            IOptions<FawaterkSettings> settings,
            ILogger<PaymentWebhooksController> logger)
        {
            _sender = sender;
            _paymentService = paymentService;
            _settings = settings.Value;
            _logger = logger;
        }

        [HttpPost("fawaterk_json")]
        public async Task<IActionResult> HandleFawaterkWebhook([FromBody] FawaterkWebhookDto webhook)
        {
            var isValid = _paymentService.VerifyWebhookSignature(
                webhook.InvoiceId.ToString(),
                webhook.InvoiceKey,
                webhook.PaymentMethod,
                _settings.VendorKey,
                webhook.HashKey);

            if (!isValid)
            {
                _logger.LogWarning("Invalid Fawaterk webhook signature for InvoiceId {InvoiceId}.", webhook.InvoiceId);
                return Unauthorized(new { message = "Invalid webhook signature." });
            }

            var command = new ProcessPaymentWebhookCommand
            {
                InvoiceId = webhook.InvoiceId,
                InvoiceKey = webhook.InvoiceKey,
                InvoiceStatus = webhook.InvoiceStatus ?? string.Empty,
                PaymentMethod = webhook.PaymentMethod,
                HashKey = webhook.HashKey,
                OrderReferenceId = webhook.OrderReferenceId
            };

            var result = await _sender.Send(command);

            if (!result)
                return NotFound(new { message = "Order not found." });

            return Ok(new { message = "Webhook processed." });
        }
    }
}


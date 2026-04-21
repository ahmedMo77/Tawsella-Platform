using MediatR;

namespace Tawsella.Application.Features.Orders.Commands.ProcessPaymentWebhook
{
    public class ProcessPaymentWebhookCommand : IRequest<bool>
    {
        public int InvoiceId { get; set; }
        public string InvoiceKey { get; set; } = string.Empty;
        public string InvoiceStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string HashKey { get; set; } = string.Empty;
        public string? OrderReferenceId { get; set; }
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Orders.Commands.ProcessPaymentWebhook
{
    public class ProcessPaymentWebhookCommandHandler : IRequestHandler<ProcessPaymentWebhookCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<ProcessPaymentWebhookCommandHandler> _logger;

        public ProcessPaymentWebhookCommandHandler(
            IOrderRepository orderRepository,
            IPaymentService paymentService,
            ILogger<ProcessPaymentWebhookCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<bool> Handle(ProcessPaymentWebhookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Fawaterk webhook. InvoiceId: {InvoiceId}, Status: {Status}",
                request.InvoiceId, request.InvoiceStatus);

            var order = await _orderRepository.GetByIdAsync(request.OrderReferenceId!);
            if (order == null)
            {
                _logger.LogWarning("Order not found for reference {OrderRef}", request.OrderReferenceId);
                return false;
            }

            var status = request.InvoiceStatus.ToLower();

            if (status == "paid" || status == "success")
            {
                await _orderRepository.UpdatePaymentStatusAsync(
                    order.Id, PaymentStatus.Completed, "Payment confirmed via Fawaterk", cancellationToken);

                _logger.LogInformation("Payment confirmed for Order {OrderId}.", order.Id);
            }
            else if (status == "failed" || status == "expired" || status == "canceled")
            {
                await _orderRepository.UpdatePaymentStatusAsync(
                    order.Id, PaymentStatus.Failed, $"Payment failed via Fawaterk ({request.InvoiceStatus})", cancellationToken);

                _logger.LogWarning("Payment failed for Order {OrderId}. Status: {Status}", order.Id, request.InvoiceStatus);
            }
            else
            {
                _logger.LogWarning("Unhandled Fawaterk status '{Status}' for InvoiceId {InvoiceId}.", request.InvoiceStatus, request.InvoiceId);
            }

            return true;
        }
    }
}

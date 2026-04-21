using Tawsella.Application.DTOs.PaymentDTOs;

namespace Tawsella.Application.Contracts.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto request);
        bool VerifyWebhookSignature(string invoiceId, string invoiceKey, string paymentMethod, string vendorKey, string hashKey);
    }
}

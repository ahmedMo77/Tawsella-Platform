namespace Tawsella.Application.DTOs.PaymentDTOs
{
    public class PaymentResponseDto
    {
        public bool Success { get; set; }
        public string? PaymentUrl { get; set; }
        public string? InvoiceId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

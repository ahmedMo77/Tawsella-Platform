using Tawsella.Domain.Entities;

namespace Tawsella.Application.DTOs.PaymentDTOs
{
    public class PaymentRequestDto
    {
        public Order Order { get; set; } = null!;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
    }
}

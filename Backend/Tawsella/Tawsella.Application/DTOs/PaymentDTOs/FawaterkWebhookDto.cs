using System.Text.Json.Serialization;

namespace Tawsella.Application.DTOs.PaymentDTOs
{
    public class FawaterkWebhookDto
    {
        [JsonPropertyName("invoice_id")]
        public int InvoiceId { get; set; }

        [JsonPropertyName("invoice_key")]
        public string InvoiceKey { get; set; } = string.Empty;

        [JsonPropertyName("invoice_status")]
        public string? InvoiceStatus { get; set; }

        [JsonPropertyName("payment_method")]
        public string PaymentMethod { get; set; } = string.Empty;

        [JsonPropertyName("hashKey")]
        public string HashKey { get; set; } = string.Empty;

        [JsonPropertyName("pay_load")]
        public string? PayLoadString { get; set; }

        public string? OrderReferenceId
        {
            get
            {
                if (!string.IsNullOrEmpty(PayLoadString))
                {
                    try
                    {
                        var obj = System.Text.Json.JsonSerializer.Deserialize<FawaterkNestedPayLoad>(PayLoadString);
                        return obj?.OrderReferenceId;
                    }
                    catch { }
                }
                return null;
            }
        }
    }

    public class FawaterkNestedPayLoad
    {
        [JsonPropertyName("order_reference_id")]
        public string? OrderReferenceId { get; set; }
    }
}

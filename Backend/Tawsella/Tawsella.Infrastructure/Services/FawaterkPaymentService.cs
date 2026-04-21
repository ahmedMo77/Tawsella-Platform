using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs.PaymentDTOs;
using Tawsella.Domain.Entities;
using Tawsella.Infrastructure.Models;

namespace Tawsella.Infrastructure.Services
{
    public class FawaterkPaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly FawaterkSettings _settings;
        private readonly ILogger<FawaterkPaymentService> _logger;

        public FawaterkPaymentService(
            HttpClient httpClient,
            IOptions<FawaterkSettings> settings,
            ILogger<FawaterkPaymentService> logger)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto request)
        {
            try
            {
                var nameParts = request.CustomerName.Split(' ', 2);
                var firstName = nameParts[0];
                var lastName = nameParts.Length > 1 ? nameParts[1] : "";

                var payload = new
                {
                    cartTotal = request.Order.Money.EstimatedPrice,
                    currency = "EGP",
                    customer = new
                    {
                        first_name = firstName,
                        last_name = lastName,
                        email = request.CustomerEmail,
                        phone = request.CustomerPhone,
                        address = request.Order.Pickup.Location?.AddressName ?? "N/A"
                    },
                    redirectionUrls = new
                    {
                        successUrl = _settings.SuccessRedirectUrl,
                        failUrl = _settings.FailRedirectUrl
                    },
                    cartItems = new[]
                    {
                        new
                        {
                            name = $"Delivery Order #{request.Order.OrderNumber}",
                            price = request.Order.Money.EstimatedPrice,
                            quantity = 1
                        }
                    },
                    payLoad = new
                    {
                        order_reference_id = request.Order.Id
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Sending payment request to Fawaterk for Order {OrderNumber}, Amount: {Amount} EGP",
                    request.Order.OrderNumber, request.Order.Money.EstimatedPrice);

                var response = await _httpClient.PostAsync("api/v2/createInvoiceLink", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Fawaterk API error. Status: {StatusCode}, Body: {Body}",
                        response.StatusCode, responseBody);

                    return new PaymentResponseDto
                    {
                        Success = false,
                        ErrorMessage = $"Payment gateway returned {response.StatusCode}. Please try again."
                    };
                }

                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                // Log the full response so we can see the exact field names Fawaterk returns
                _logger.LogInformation("Fawaterk full response: {Body}", responseBody);

                var status = root.GetProperty("status").GetString();

                if (status?.ToLower() == "success" && root.TryGetProperty("data", out var data))
                {
                    var paymentUrl = data.GetProperty("url").GetString();

                   
                    string? invoiceId = null;
                    if (data.TryGetProperty("invoiceId", out var invId)) invoiceId = invId.ToString();

                    _logger.LogInformation("Fawaterk invoice created. InvoiceId: {InvoiceId}, URL: {Url}",
                        invoiceId, paymentUrl);

                    return new PaymentResponseDto
                    {
                        Success = true,
                        PaymentUrl = paymentUrl,
                        InvoiceId = invoiceId
                    };
                }

                _logger.LogWarning("Fawaterk returned unexpected response: {Body}", responseBody);
                return new PaymentResponseDto
                {
                    Success = false,
                    ErrorMessage = "Unexpected response from payment gateway."
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error while communicating with Fawaterk");
                return new PaymentResponseDto
                {
                    Success = false,
                    ErrorMessage = "Could not reach payment gateway. Please try again later."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during payment creation for Order {OrderId}", request.Order.Id);
                return new PaymentResponseDto
                {
                    Success = false,
                    ErrorMessage = "An unexpected error occurred during payment processing."
                };
            }
        }

        public bool VerifyWebhookSignature(string invoiceId, string invoiceKey, string paymentMethod, string vendorKey, string hashKey)
        {
            var queryParam = $"InvoiceId={invoiceId}&InvoiceKey={invoiceKey}&PaymentMethod={paymentMethod}";
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(vendorKey);
            var dataBytes = System.Text.Encoding.UTF8.GetBytes(queryParam);

            using var hmac = new System.Security.Cryptography.HMACSHA256(keyBytes);
            var computedHash = hmac.ComputeHash(dataBytes);
            var computedHashHex = BitConverter.ToString(computedHash).Replace("-", "").ToLower();

            return string.Equals(computedHashHex, hashKey, StringComparison.OrdinalIgnoreCase);
        }
    }
}

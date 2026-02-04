using System;
using Tawsella.Application.DTOs.CourierDTOs;

namespace Tawsella.Domain.DTOs.OrderDTOs
{
    /// <summary>
    /// Application for an order including courier public profile (for customer to choose).
    /// </summary>
    public class OrderApplicationWithCourierDto
    {
        public string ApplicationId { get; set; }
        public string OrderId { get; set; }
        public string CourierId { get; set; }
        public DateTime AppliedAt { get; set; }
        public CourierPublicProfileDto Courier { get; set; }
    }
}

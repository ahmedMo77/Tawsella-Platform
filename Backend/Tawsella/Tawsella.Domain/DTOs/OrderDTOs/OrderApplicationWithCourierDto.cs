using System;
using Tawsella.Application.DTOs.CourierDTOs;

namespace Tawsella.Domain.DTOs.OrderDTOs
{
    public class OrderApplicationWithCourierDto
    {
        public string ApplicationId { get; set; }
        public string OrderId { get; set; }
        public string CourierId { get; set; }
        public DateTime AppliedAt { get; set; }
        public CourierPublicProfileDto Courier { get; set; }
    }
}

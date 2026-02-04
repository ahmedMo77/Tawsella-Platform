using System;

namespace Tawsella.Domain.DTOs.ReviewDTOs
{
    public class ReviewDto
    {
        public string Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CourierId { get; set; }
        public string CourierName { get; set; }
    }
}

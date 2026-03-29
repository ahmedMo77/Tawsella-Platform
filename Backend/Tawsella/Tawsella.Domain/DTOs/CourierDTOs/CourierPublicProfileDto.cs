using System.Collections.Generic;
using Tawsella.Domain.DTOs.CourierDTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.DTOs.CourierDTOs
{
    /// <summary>
    /// Public profile for customers to see when viewing applicants (rating, reviews, vehicle).
    /// </summary>
    public class CourierPublicProfileDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string VehicleType { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int CompletedDeliveries { get; set; }
        public List<CourierReviewItemDto> Reviews { get; set; }
    }
}

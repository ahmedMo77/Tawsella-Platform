using Tawsella.Domain.DTOs.CourierDTOs;

namespace Tawsella.Application.Features.Couriers.Queries.GetRecentReviews
{
    public class GetRecentReviewsQueryResponse
    {
        public List<CourierReviewItemDto> Reviews { get; set; }
    }
}

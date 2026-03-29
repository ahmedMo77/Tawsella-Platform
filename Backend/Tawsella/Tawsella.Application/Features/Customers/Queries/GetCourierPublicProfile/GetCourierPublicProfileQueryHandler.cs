using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Customers.Queries.GetCourierPublicProfile
{
    public class GetCourierPublicProfileQueryHandler 
        : IRequestHandler<GetCourierPublicProfileQuery, GetCourierPublicProfileQueryResponse>
    {
        private readonly ICourierRepository _courierRepository;

        public GetCourierPublicProfileQueryHandler(ICourierRepository courierRepository)
        {
            _courierRepository = courierRepository;
        }

        public async Task<GetCourierPublicProfileQueryResponse> Handle(
            GetCourierPublicProfileQuery request, 
            CancellationToken cancellationToken)
        {
            var courier = await _courierRepository.GetCourierPublicProfileAsync(request.CourierId, cancellationToken);

            if (courier == null)
                throw new KeyNotFoundException("Courier not found.");

            // Get reviews - using EF Core directly for now as this is a complex DTO projection
            // Could be moved to repository if this pattern is used frequently
            var reviews = await _courierRepository.GetCourierRecentReviewsAsync(
                request.CourierId, 
                50, 
                cancellationToken);

            var reviewDtos = reviews.Select(r => new CourierReviewItemDto
            {
                Rating = r.Rating,
                Comment = r.Comment ?? "",
                CreatedAt = r.CreatedAt
            }).ToList();

            var (totalDeliveries, _, averageRating) = await _courierRepository.GetCourierDashboardStatsAsync(
                request.CourierId, 
                cancellationToken);

            return new GetCourierPublicProfileQueryResponse
            {
                Profile = new CourierPublicProfileDto
                {
                    Id = courier.Id,
                    FullName = courier.User?.FullName ?? "",
                    VehicleType = courier.VehicleType,
                    AverageRating = averageRating,
                    TotalReviews = reviews.Count,
                    CompletedDeliveries = totalDeliveries,
                    Reviews = reviewDtos
                }
            };
        }
    }
}

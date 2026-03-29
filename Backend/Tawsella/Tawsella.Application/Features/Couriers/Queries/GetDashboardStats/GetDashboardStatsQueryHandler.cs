using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Application.DTOs.CourierDTOs;

namespace Tawsella.Application.Features.Couriers.Queries.GetDashboardStats
{
    public class GetDashboardStatsQueryHandler 
        : IRequestHandler<GetDashboardStatsQuery, GetDashboardStatsQueryResponse>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetDashboardStatsQueryHandler(
            ICourierRepository courierRepository,
            ICurrentUserService currentUserService)
        {
            _courierRepository = courierRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetDashboardStatsQueryResponse> Handle(
            GetDashboardStatsQuery request, 
            CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var (totalDeliveries, totalEarnings, averageRating) = await _courierRepository.GetCourierDashboardStatsAsync(
                courierId, 
                cancellationToken);

            if (totalDeliveries == 0 && totalEarnings == 0 && averageRating == 0)
            {
                return new GetDashboardStatsQueryResponse
                {
                    Stats = null
                };
            }

            return new GetDashboardStatsQueryResponse
            {
                Stats = new CourierStatsDto
                {
                    AverageRating = averageRating,
                    TotalReviews = 0, // Can be enhanced if needed
                    CompletedDeliveries = totalDeliveries
                }
            };
        }
    }
}

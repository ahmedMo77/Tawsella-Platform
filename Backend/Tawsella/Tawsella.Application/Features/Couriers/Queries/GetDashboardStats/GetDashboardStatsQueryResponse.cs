using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Domain.DTOs.CourierDTOs;

namespace Tawsella.Application.Features.Couriers.Queries.GetDashboardStats
{
    public class GetDashboardStatsQueryResponse
    {
        public CourierStatsDto Stats { get; set; }
    }
}

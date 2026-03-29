using MediatR;

namespace Tawsella.Application.Features.Couriers.Queries.GetDashboardStats
{
    public record GetDashboardStatsQuery() : IRequest<GetDashboardStatsQueryResponse>;
}

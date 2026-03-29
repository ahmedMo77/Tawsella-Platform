using MediatR;

namespace Tawsella.Application.Features.Couriers.Queries.GetAvailableOrders
{
    public record GetAvailableOrdersQuery(double RadiusInKm = 10) 
        : IRequest<GetAvailableOrdersQueryResponse>;
}

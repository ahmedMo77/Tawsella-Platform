using MediatR;

namespace Tawsella.Application.Features.Couriers.Queries.GetAvailableOrders
{
    public record GetAvailableOrdersQuery(double RadiusInKm) 
        : IRequest<GetAvailableOrdersQueryResponse>;
}

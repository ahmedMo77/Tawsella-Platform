using MediatR;

namespace Tawsella.Application.Features.Couriers.Queries.GetAvailableOrders
{
    public class GetAvailableOrdersQuery : IRequest<GetAvailableOrdersQueryResponse>
    {
        public double RadiusInKm { get; set; } = 10;
    }
}

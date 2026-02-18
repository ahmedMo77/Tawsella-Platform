using MediatR;

namespace Tawsella.Application.Features.Couriers.Commands.PickupOrder
{
    public class PickupOrderCommand : IRequest<PickupOrderCommandResponse>
    {
        public string OrderId { get; set; }
    }
}

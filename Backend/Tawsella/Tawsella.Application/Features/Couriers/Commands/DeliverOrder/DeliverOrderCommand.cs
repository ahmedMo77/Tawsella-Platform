using MediatR;

namespace Tawsella.Application.Features.Couriers.Commands.DeliverOrder
{
    public class DeliverOrderCommand : IRequest<DeliverOrderCommandResponse>
    {
        public string OrderId { get; set; }
    }
}

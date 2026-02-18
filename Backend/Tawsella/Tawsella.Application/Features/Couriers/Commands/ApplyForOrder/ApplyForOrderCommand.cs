using MediatR;

namespace Tawsella.Application.Features.Couriers.Commands.ApplyForOrder
{
    public class ApplyForOrderCommand : IRequest<ApplyForOrderCommandResponse>
    {
        public string OrderId { get; set; }
    }
}

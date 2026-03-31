using MediatR;
using Tawsella.Application.DTOs;

namespace Tawsella.Application.Features.Couriers.Commands.DeliverOrder
{
    public class DeliverOrderCommand : IRequest<BaseToReturnDto>
    {
        public string OrderId { get; set; }
    }
}

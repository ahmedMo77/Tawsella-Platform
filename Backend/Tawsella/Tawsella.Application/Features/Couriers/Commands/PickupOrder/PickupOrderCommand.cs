using MediatR;
using Tawsella.Application.DTOs;

namespace Tawsella.Application.Features.Couriers.Commands.PickupOrder
{
    public class PickupOrderCommand : IRequest<BaseToReturnDto>
    {
        public string OrderId { get; set; }
    }
}

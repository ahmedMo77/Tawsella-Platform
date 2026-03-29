using Tawsella.Application.DTOs.OrderDTOs;

namespace Tawsella.Application.Features.Couriers.Queries.GetAvailableOrders
{
    public class GetAvailableOrdersQueryResponse
    {
        public List<OrderResponseDto> Orders { get; set; }
    }
}

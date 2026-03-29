using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.OrderDTOs;

namespace Tawsella.Application.Features.Orders.Queries.GetOrdersHistory
{
    public class GetOrdersHistoryQueryResponse
    {
        public PaginatedResponseDto<OrderResponseDto> paginatedResponse {  get; set; } = default!;
    }
}

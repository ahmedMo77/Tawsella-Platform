using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.DTOs.OrderDTOs;

namespace Tawsella.Application.Features.Orders.Queries.GetOrdersHistory
{
    public class GetOrdersHistoryQueryResponse
    {
        public PaginatedResponseDto<OrderResponseDto> paginatedResponse {  get; set; } = default!;
    }
}

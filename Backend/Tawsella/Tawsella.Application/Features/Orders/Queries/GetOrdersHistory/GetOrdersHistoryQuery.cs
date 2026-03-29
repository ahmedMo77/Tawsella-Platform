using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Orders.Queries.GetOrdersHistory
{
    public record GetOrdersHistoryQuery(
        OrderStatus? Status = null,
        int Page = 1,
        int PageSize = 10
    ) : IRequest<GetOrdersHistoryQueryResponse>;
}

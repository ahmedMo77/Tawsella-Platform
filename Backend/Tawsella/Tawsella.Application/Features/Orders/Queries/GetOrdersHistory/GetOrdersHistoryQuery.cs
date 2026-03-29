using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Enums;

namespace Tawsella.Application.Features.Orders.Queries.GetOrdersHistory
{
    public class GetOrdersHistoryQuery:IRequest<GetOrdersHistoryQueryResponse>
    {
        public OrderStatus? status { get; set; }
        public int page {  get; set; }
        public int pageSize { get; set; }

    }
}

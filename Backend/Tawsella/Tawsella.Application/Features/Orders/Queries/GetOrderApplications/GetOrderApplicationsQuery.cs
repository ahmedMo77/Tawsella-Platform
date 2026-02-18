using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Features.Couriers.Queries.GetActiveOrder;

namespace Tawsella.Application.Features.Orders.Queries.GetOrderApplications
{
    public class GetOrderApplicationsQuery:IRequest<List<GetOrderApplicationsResponse>>
    {
        public string orderId { get; set; }
    }
}

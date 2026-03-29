using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Enums;

namespace Tawsella.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommand:IRequest<UpdateOrderStatusResponse>
    {
        public string OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
        public string Notes { get; set; }
    }
}

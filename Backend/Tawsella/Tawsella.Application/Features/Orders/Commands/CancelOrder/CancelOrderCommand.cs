using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand:IRequest<CancelOrderCommandResponse>
    {
      public string orderId {  get; set; }
      public string reason { get; set; }
    }
}

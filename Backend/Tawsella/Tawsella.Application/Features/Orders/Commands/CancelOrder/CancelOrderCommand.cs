using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;

namespace Tawsella.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommand:IRequest<BaseToReturnDto>
    {
      public string OrderId {  get; set; }
      public string Reason { get; set; }
    }
}

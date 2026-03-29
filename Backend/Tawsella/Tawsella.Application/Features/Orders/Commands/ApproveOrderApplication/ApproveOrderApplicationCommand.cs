using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;

namespace Tawsella.Application.Features.Orders.Commands.ApproveOrderApplication
{
    public class ApproveOrderApplicationCommand:IRequest<BaseToReturnDto>
    {
       public string orderId { get; set; }
        
       public string applicationId { get; set; }

    }
}

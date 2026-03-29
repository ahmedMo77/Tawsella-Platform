using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;

namespace Tawsella.Application.Features.Orders.Commands.ApproveOrderApplication
{
    public record ApproveOrderApplicationCommand(
        string OrderId,
        string ApplicationId
    ) : IRequest<BaseToReturnDto>;

}

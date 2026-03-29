using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.DTOs;

namespace Tawsella.Application.Features.Admin.Commands.CreateAdmin
{
    public record CreateAdminCommand(
        string FullName,
        string Email,
        bool IsSuperAdmin
    ) : IRequest<BaseToReturnDto>;

}

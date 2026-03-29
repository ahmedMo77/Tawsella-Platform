using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;

namespace Tawsella.Application.Features.Auth.Password.ChangePassword
{
    public record ChangePasswordCommand(
        string UserId, 
        string OldPassword, 
        string NewPassword
        ) : IRequest<BaseToReturnDto>;
}

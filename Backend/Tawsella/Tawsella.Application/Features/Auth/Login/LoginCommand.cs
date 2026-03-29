using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.AuthDTOS;

namespace Tawsella.Application.Features.Auth.Login
{
    public record LoginCommand(
        string Email, 
        string Password
        ) : IRequest<AuthResultDto>;
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.DTOs;

namespace Tawsella.Application.Features.Auth.Password.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest<BaseToReturnDto>;
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;

namespace Tawsella.Application.Features.Auth.Register.RegisterCustomer
{
    public record RegisterCustomerCommand(
        string FullName,
        string Email,
        string Password,
        string PhoneNumber
    ) : IRequest<BaseToReturnDto>;
}

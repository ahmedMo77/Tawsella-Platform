using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Tawsella.Application.Features.Authentication.Commands.RegisterCustomer
{
    public class RegisterCustomerCommand : IRequest<RegisterCustomerCommandResponse>
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}

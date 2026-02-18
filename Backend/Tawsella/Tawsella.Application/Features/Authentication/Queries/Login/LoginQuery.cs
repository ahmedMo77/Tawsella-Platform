using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Tawsella.Application.Features.Authentication.Queries.Login
{
    public class LoginQuery : IRequest<LoginQueryResponse>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

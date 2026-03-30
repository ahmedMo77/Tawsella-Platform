using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.Features.Auth.Register.RegisterCustomer
{
    public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
    {
        public RegisterCustomerCommandValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^01[0125]\d{8}$")
                .WithMessage("Please enter a valid Egyptian phone number.");
        }
    }
}

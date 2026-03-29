using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.Features.Auth.Register.RegisterCourier
{
    public class RegisterCourierCommandValidator : AbstractValidator<RegisterCourierCommand>
    {
        public RegisterCourierCommandValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\d{11}$").WithMessage("Phone number must be 11 digits.");

            RuleFor(x => x.NationalId)
                .NotEmpty()
                .Matches(@"^\d{14}$").WithMessage("National ID must be 14 digits.");

            RuleFor(x => x.VehiclePlateNumber).NotEmpty();
            RuleFor(x => x.LicenseExpiryDate).GreaterThan(DateTime.UtcNow)
                .WithMessage("Your license must be valid.");
        }
    }
}

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
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^01[0125]\d{8}$")
                .WithMessage("Invalid Egyptian phone number format.");

            // الرقم القومي (14 رقم ويبدأ بـ 2 أو 3)
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .Matches(@"^[23]\d{13}$")
                .WithMessage("National ID must be 14 digits and start with 2 or 3.");

            RuleFor(x => x.VehiclePlateNumber).NotEmpty();
            RuleFor(x => x.LicenseExpiryDate).GreaterThan(DateTime.UtcNow)
                .WithMessage("Your license must be valid.");
        }
    }
}

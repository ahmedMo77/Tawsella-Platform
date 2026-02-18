using FluentValidation;

namespace Tawsella.Application.Features.Customers.Commands.UpdateCustomerProfile
{
    public class UpdateCustomerProfileCommandValidator : AbstractValidator<UpdateCustomerProfileCommand>
    {
        public UpdateCustomerProfileCommandValidator()
        {
            RuleFor(x => x.DefaultPickupAddress)
                .MaximumLength(500).WithMessage("Default pickup address must not exceed 500 characters.")
                .When(x => x.DefaultPickupAddress != null);

            RuleFor(x => x.DefaultPickupLatitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.")
                .When(x => x.DefaultPickupLatitude.HasValue);

            RuleFor(x => x.DefaultPickupLongitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.")
                .When(x => x.DefaultPickupLongitude.HasValue);

            RuleFor(x => x.DefaultPickupLabel)
                .MaximumLength(50).WithMessage("Pickup label must not exceed 50 characters.")
                .When(x => x.DefaultPickupLabel != null);

            RuleFor(x => x.PreferredPaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method.")
                .When(x => x.PreferredPaymentMethod.HasValue);
        }
    }
}

using FluentValidation;

namespace Tawsella.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.PickupAddress)
                .NotEmpty().WithMessage("Pickup address is required.")
                .MaximumLength(500).WithMessage("Pickup address must not exceed 500 characters.");

            RuleFor(x => x.PickupLatitude)
                .InclusiveBetween(-90, 90).WithMessage("Pickup latitude must be between -90 and 90.");

            RuleFor(x => x.PickupLongitude)
                .InclusiveBetween(-180, 180).WithMessage("Pickup longitude must be between -180 and 180.");

            RuleFor(x => x.PickupContactName)
                .NotEmpty().WithMessage("Pickup contact name is required.")
                .MaximumLength(100).WithMessage("Pickup contact name must not exceed 100 characters.");

            RuleFor(x => x.PickupContactPhone)
                .NotEmpty().WithMessage("Pickup contact phone is required.")
                .MaximumLength(20).WithMessage("Pickup contact phone must not exceed 20 characters.")
                .Matches(@"^\+?[0-9\s\-\(\)]{7,20}$").WithMessage("Pickup contact phone is not valid.");

            RuleFor(x => x.DropoffAddress)
                .NotEmpty().WithMessage("Dropoff address is required.")
                .MaximumLength(500).WithMessage("Dropoff address must not exceed 500 characters.");

            RuleFor(x => x.DropoffLatitude)
                .InclusiveBetween(-90, 90).WithMessage("Dropoff latitude must be between -90 and 90.");

            RuleFor(x => x.DropoffLongitude)
                .InclusiveBetween(-180, 180).WithMessage("Dropoff longitude must be between -180 and 180.");

            RuleFor(x => x.DropoffContactName)
                .NotEmpty().WithMessage("Dropoff contact name is required.")
                .MaximumLength(100).WithMessage("Dropoff contact name must not exceed 100 characters.");

            RuleFor(x => x.DropoffContactPhone)
                .NotEmpty().WithMessage("Dropoff contact phone is required.")
                .MaximumLength(20).WithMessage("Dropoff contact phone must not exceed 20 characters.")
                .Matches(@"^\+?[0-9\s\-\(\)]{7,20}$").WithMessage("Dropoff contact phone is not valid.");

            RuleFor(x => x.PackageSize)
                .NotEmpty().WithMessage("Package size is required.")
                .MaximumLength(50).WithMessage("Package size must not exceed 50 characters.")
                .Must(s => s == "Small" || s == "Medium" || s == "Large")
                .WithMessage("Package size must be Small, Medium, or Large.");

            RuleFor(x => x.PackageWeight)
                .GreaterThan(0).WithMessage("Package weight must be greater than 0.")
                .When(x => x.PackageWeight.HasValue);

            RuleFor(x => x.PackageNotes)
                .MaximumLength(500).WithMessage("Package notes must not exceed 500 characters.")
                .When(x => x.PackageNotes != null);

            RuleFor(x => x.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method.");
        }
    }
}

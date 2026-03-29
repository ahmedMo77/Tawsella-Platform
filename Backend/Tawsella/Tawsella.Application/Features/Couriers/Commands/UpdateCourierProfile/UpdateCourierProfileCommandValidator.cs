using FluentValidation;

namespace Tawsella.Application.Features.Couriers.Commands.UpdateCourierProfile
{
    public class UpdateCourierProfileCommandValidator : AbstractValidator<UpdateCourierProfileCommand>
    {
        public UpdateCourierProfileCommandValidator()
        {
            RuleFor(x => x.FullName)
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.")
                .When(x => x.FullName != null);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
                .Matches(@"^\+?[0-9\s\-\(\)]{7,20}$").WithMessage("Phone number is not valid.")
                .When(x => x.PhoneNumber != null);

            RuleFor(x => x.VehicleType)
                .MaximumLength(50).WithMessage("Vehicle type must not exceed 50 characters.")
                .When(x => x.VehicleType != null);

            RuleFor(x => x.VehiclePlateNumber)
                .MaximumLength(20).WithMessage("Vehicle plate number must not exceed 20 characters.")
                .When(x => x.VehiclePlateNumber != null);
        }
    }
}

using FluentValidation;

namespace Tawsella.Application.Features.Couriers.Commands.PickupOrder
{
    public class PickupOrderCommandValidator : AbstractValidator<PickupOrderCommand>
    {
        public PickupOrderCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Order ID is required.");
        }
    }
}

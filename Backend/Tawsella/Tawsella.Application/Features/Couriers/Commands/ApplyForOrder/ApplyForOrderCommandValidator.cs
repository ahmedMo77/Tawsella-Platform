using FluentValidation;

namespace Tawsella.Application.Features.Couriers.Commands.ApplyForOrder
{
    public class ApplyForOrderCommandValidator : AbstractValidator<ApplyForOrderCommand>
    {
        public ApplyForOrderCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Order ID is required.");
        }
    }
}

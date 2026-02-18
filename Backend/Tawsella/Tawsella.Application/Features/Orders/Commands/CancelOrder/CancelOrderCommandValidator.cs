using FluentValidation;

namespace Tawsella.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(x => x.orderId)
                .NotEmpty().WithMessage("Order ID is required.");

            RuleFor(x => x.reason)
                .NotEmpty().WithMessage("Cancellation reason is required.")
                .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
        }
    }
}

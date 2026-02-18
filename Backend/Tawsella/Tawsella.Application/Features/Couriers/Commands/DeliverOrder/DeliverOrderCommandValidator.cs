using FluentValidation;

namespace Tawsella.Application.Features.Couriers.Commands.DeliverOrder
{
    public class DeliverOrderCommandValidator : AbstractValidator<DeliverOrderCommand>
    {
        public DeliverOrderCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Order ID is required.");
        }
    }
}

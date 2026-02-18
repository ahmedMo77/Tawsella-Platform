using FluentValidation;

namespace Tawsella.Application.Features.Orders.Commands.ApproveOrderApplication
{
    public class ApproveOrderApplicationCommandValidator : AbstractValidator<ApproveOrderApplicationCommand>
    {
        public ApproveOrderApplicationCommandValidator()
        {
            RuleFor(x => x.orderId)
                .NotEmpty().WithMessage("Order ID is required.");

            RuleFor(x => x.applicationId)
                .NotEmpty().WithMessage("Application ID is required.");
        }
    }
}

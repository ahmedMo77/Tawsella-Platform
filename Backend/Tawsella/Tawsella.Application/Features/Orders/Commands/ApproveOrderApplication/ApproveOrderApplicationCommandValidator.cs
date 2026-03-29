using FluentValidation;

namespace Tawsella.Application.Features.Orders.Commands.ApproveOrderApplication
{
    public class ApproveOrderApplicationCommandValidator : AbstractValidator<ApproveOrderApplicationCommand>
    {
        public ApproveOrderApplicationCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Order ID is required.");

            RuleFor(x => x.ApplicationId)
                .NotEmpty().WithMessage("Application ID is required.");
        }
    }
}

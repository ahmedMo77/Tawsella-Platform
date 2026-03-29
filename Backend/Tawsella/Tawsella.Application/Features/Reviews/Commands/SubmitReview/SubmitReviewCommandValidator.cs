using FluentValidation;

namespace Tawsella.Application.Features.Reviews.Commands.SubmitReview
{
    public class SubmitReviewCommandValidator : AbstractValidator<SubmitReviewCommand>
    {
        public SubmitReviewCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Order ID is required.");

            RuleFor(x => x.Dto)
                .NotNull().WithMessage("Review data is required.");

            When(x => x.Dto != null, () =>
            {
                RuleFor(x => x.Dto.Rating)
                    .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

                RuleFor(x => x.Dto.Comment)
                    .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters.")
                    .When(x => x.Dto.Comment != null);
            });
        }
    }
}

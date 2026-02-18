using FluentValidation;

namespace Tawsella.Application.Features.Notifications.Commands.SendNotification
{
    public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
    {
        public SendNotificationCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Notification title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Notification message is required.")
                .MaximumLength(1000).WithMessage("Message must not exceed 1000 characters.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid notification type.");
        }
    }
}

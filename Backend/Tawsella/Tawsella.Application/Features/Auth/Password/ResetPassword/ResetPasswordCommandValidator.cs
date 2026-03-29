using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.Features.Auth.Password.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator() 
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Reset code is required.");
            RuleFor(x => x.NewPassword)
                .NotEmpty().MinimumLength(8)
                .Matches(@"[A-Z]").WithMessage("Must contain uppercase")
                .Matches(@"[0-9]").WithMessage("Must contain number");
        }
    }
}

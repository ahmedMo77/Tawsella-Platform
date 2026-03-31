using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.Features.Auth.Password.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Old password is required.");
            RuleFor(x => x.NewPassword)
                .NotEmpty().MinimumLength(8)
                .Matches(@"[A-Z]").WithMessage("Must contain uppercase")
                .Matches(@"[0-9]").WithMessage("Must contain number");
        }
    }
}

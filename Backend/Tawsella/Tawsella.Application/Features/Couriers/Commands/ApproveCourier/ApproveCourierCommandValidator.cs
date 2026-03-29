using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.Features.Couriers.Commands.ApproveCourier
{
    public class ApproveCourierCommandValidator : AbstractValidator<ApproveCourierCommand>
    {
        public ApproveCourierCommandValidator()
        {
            RuleFor(x => x.CourierId)
                .NotEmpty().WithMessage("Courier ID is required.");
        }
    }
}

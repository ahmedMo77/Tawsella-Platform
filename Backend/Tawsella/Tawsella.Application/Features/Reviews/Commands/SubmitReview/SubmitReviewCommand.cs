using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.ReviewDTOs;

namespace Tawsella.Application.Features.Reviews.Commands.SubmitReview
{
    public record SubmitReviewCommand(
        string OrderId, 
        CreateReviewDto Dto
    ) : IRequest<BaseToReturnDto>;

}

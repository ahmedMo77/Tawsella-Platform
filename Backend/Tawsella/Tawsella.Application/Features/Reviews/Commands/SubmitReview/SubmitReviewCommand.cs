using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.ReviewDTOs;

namespace Tawsella.Application.Features.Reviews.Commands.SubmitReview
{
    public class SubmitReviewCommand:IRequest<SubmitReviewCommandResponse>
    {
       public string orderId { get; set; }
       public CreateReviewDto dto { get; set; }
    }
}

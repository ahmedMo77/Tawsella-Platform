using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.Features.Reviews.Queries.GetOrderReview
{
    public class GetOrderReviewQuery : IRequest<GetOrderReviewQueryResponse>
    {
        public string orderId { get; set; }
    }
}

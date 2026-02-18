using MediatR;

namespace Tawsella.Application.Features.Couriers.Queries.GetRecentReviews
{
    public class GetRecentReviewsQuery : IRequest<GetRecentReviewsQueryResponse>
    {
        public int Count { get; set; } = 5;
    }
}

using MediatR;

namespace Tawsella.Application.Features.Couriers.Queries.GetRecentReviews
{
    public record GetRecentReviewsQuery(int Count = 5) 
        : IRequest<GetRecentReviewsQueryResponse>;

}

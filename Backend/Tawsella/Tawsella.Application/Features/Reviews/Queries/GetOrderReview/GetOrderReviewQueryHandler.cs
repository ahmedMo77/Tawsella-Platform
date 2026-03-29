using AutoMapper;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Domain.DTOs.ReviewDTOs;

namespace Tawsella.Application.Features.Reviews.Queries.GetOrderReview
{
    public class GetOrderReviewQueryHandler : IRequestHandler<GetOrderReviewQuery, GetOrderReviewQueryResponse>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetOrderReviewQueryHandler(
            IReviewRepository reviewRepository, 
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GetOrderReviewQueryResponse> Handle(GetOrderReviewQuery request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();
            
            if (string.IsNullOrEmpty(request.orderId))
                throw new ValidationException("Invalid order ID");

            var review = await _reviewRepository.GetOrderReviewAsync(
                request.orderId, 
                customerId, 
                cancellationToken);

            var reviewDto = _mapper.Map<ReviewDto>(review);

            return new GetOrderReviewQueryResponse { Review = reviewDto };
        }
    }
}

using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs.CourierDTOs;

namespace Tawsella.Application.Features.Couriers.Queries.GetRecentReviews
{
    public class GetRecentReviewsQueryHandler 
        : IRequestHandler<GetRecentReviewsQuery, GetRecentReviewsQueryResponse>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetRecentReviewsQueryHandler(
            ICourierRepository courierRepository, 
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _courierRepository = courierRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GetRecentReviewsQueryResponse> Handle(
            GetRecentReviewsQuery request, 
            CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var reviews = await _courierRepository.GetCourierRecentReviewsAsync(
                courierId, 
                request.Count, 
                cancellationToken);

            return new GetRecentReviewsQueryResponse
            {
                Reviews = _mapper.Map<List<CourierReviewItemDto>>(reviews)
            };
        }
    }
}

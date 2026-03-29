using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs.OrderDTOs;

namespace Tawsella.Application.Features.Couriers.Queries.GetAvailableOrders
{
    public class GetAvailableOrdersQueryHandler 
        : IRequestHandler<GetAvailableOrdersQuery, GetAvailableOrdersQueryResponse>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IMapper _mapper;
        private readonly IPricingService _pricingService;
        private readonly ICurrentUserService _currentUserService;

        public GetAvailableOrdersQueryHandler(
            ICourierRepository courierRepository, 
            IMapper mapper, 
            IPricingService pricingService,
            ICurrentUserService currentUserService)
        {
            _courierRepository = courierRepository;
            _mapper = mapper;
            _pricingService = pricingService;
            _currentUserService = currentUserService;
        }

        public async Task<GetAvailableOrdersQueryResponse> Handle(
            GetAvailableOrdersQuery request, 
            CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            // Get available orders using repository - it handles courier validation and filtering
            var (orders, totalCount) = await _courierRepository.GetAvailableOrdersAsync(
                courierId,
                0, // latitude - not used in current implementation
                0, // longitude - not used in current implementation  
                1, // page
                1000, // large page size to get all
                cancellationToken);

            // Filter by distance using pricing service
            var courier = await _courierRepository.GetByIdAsync(courierId, cancellationToken);
            if (courier == null || !courier.IsOnline || courier.CurrentLatitude == null)
            {
                return new GetAvailableOrdersQueryResponse
                {
                    Orders = new List<OrderResponseDto>()
                };
            }

            var nearbyOrders = orders.Where(order =>
            {
                var distance = _pricingService.CalculateDistance(
                    courier.CurrentLatitude.Value,
                    courier.CurrentLongitude.Value,
                    order.PickupLatitude,
                    order.PickupLongitude
                );
                return distance <= request.RadiusInKm;
            }).ToList();

            return new GetAvailableOrdersQueryResponse
            {
                Orders = _mapper.Map<List<OrderResponseDto>>(nearbyOrders)
            };
        }
    }
}

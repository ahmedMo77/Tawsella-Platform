using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs.OrderDTOs;
    
namespace Tawsella.Application.Features.Couriers.Queries.GetActiveOrder
{
    public class GetActiveOrderQueryHandler 
        : IRequestHandler<GetActiveOrderQuery, GetActiveOrderQueryResponse>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetActiveOrderQueryHandler(
            ICourierRepository courierRepository, 
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _courierRepository = courierRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GetActiveOrderQueryResponse> Handle(
            GetActiveOrderQuery request, 
            CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var activeOrder = await _courierRepository.GetActiveCourierOrderAsync(courierId, cancellationToken);

            if (activeOrder == null)
            {
                return new GetActiveOrderQueryResponse
                {
                    Order = null
                };
            }

            return new GetActiveOrderQueryResponse
            {
                Order = _mapper.Map<OrderResponseDto>(activeOrder)
            };
        }
    }
}

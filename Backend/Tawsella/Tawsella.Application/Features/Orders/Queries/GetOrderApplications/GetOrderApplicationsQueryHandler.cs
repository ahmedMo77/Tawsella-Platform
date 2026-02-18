using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs.OrderDTOs;

namespace Tawsella.Application.Features.Orders.Queries.GetOrderApplications
{
    public class GetOrderApplicationsQueryHandler : IRequestHandler<GetOrderApplicationsQuery, List<GetOrderApplicationsResponse>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        
        public GetOrderApplicationsQueryHandler(
            IOrderRepository orderRepository, 
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<List<GetOrderApplicationsResponse>> Handle(GetOrderApplicationsQuery request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();
            
            var applications = await _orderRepository.GetOrderApplicationsAsync(
                request.orderId, 
                customerId, 
                cancellationToken);

            return _mapper.Map<List<GetOrderApplicationsResponse>>(applications);
        }
    }
}

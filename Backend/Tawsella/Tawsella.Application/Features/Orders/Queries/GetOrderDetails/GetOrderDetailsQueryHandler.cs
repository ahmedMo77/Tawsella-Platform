using AutoMapper;
using FluentValidation;
using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Domain.DTOs.OrderDTOs;

namespace Tawsella.Application.Features.Orders.Queries.GetOrderDetails
{
    public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, GetOrderDetailsQueryResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        
        public GetOrderDetailsQueryHandler(
            IOrderRepository orderRepository, 
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<GetOrderDetailsQueryResponse> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            
            if(string.IsNullOrEmpty(request.orderId))
                throw new ValidationException("Invalid order ID");

            var order = await _orderRepository.GetOrderWithDetailsAsync(request.orderId, userId, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            return _mapper.Map<GetOrderDetailsQueryResponse>(order);
        }
    }
}

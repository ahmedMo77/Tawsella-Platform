using AutoMapper;
using FluentValidation;
using MediatR;
using Tawsella.Application.DTOs;
using Tawsella.Application.Entities;
using Tawsella.Application.DTOs.OrderDTOs;
using Tawsella.Application.Enums;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;

namespace Tawsella.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand,CreateOrderCommandResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IPricingService _pricingService;
        private readonly ICurrentUserService _currentUserService;
        
        public CreateOrderCommandHandler(
            IOrderRepository orderRepository, 
            IMapper mapper, 
            IPricingService pricingService, 
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _pricingService = pricingService;
            _currentUserService = currentUserService;
        }
        public async Task<CreateOrderCommandResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();

            var customerExists = await _orderRepository.CustomerExistsAsync(customerId, cancellationToken);

            if (!customerExists) return new CreateOrderCommandResponse { Message = "Customer not found" };

            var CalculatePriceParameter = _mapper.Map<CalculatePriceDto>(request);
            var priceEstimate = _pricingService.CalculateOrderPrice(CalculatePriceParameter);

            var order = _mapper.Map<Order>(request);
            order.Id = Guid.NewGuid().ToString();
            order.OrderNumber = $"TW-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..5].ToUpper()}";
            order.UserId = customerId;
            order.Status = OrderStatus.Pending;
            order.EstimatedPrice = priceEstimate.EstimatedPrice;
            order.CourierEarnings = priceEstimate.CourierEarnings;
            order.PlatformCommission = priceEstimate.PlatformCommission;

            await _orderRepository.AddAsync(order, cancellationToken);
            await _orderRepository.AddStatusHistoryAsync(order.Id, OrderStatus.Pending, "Order created");

            return new CreateOrderCommandResponse { Success = true, Message = $"Order created. ID: {order.OrderNumber}" };
        }

        
    }
}

using AutoMapper;
using FluentValidation;
using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.DTOs.OrderDTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand,CreateOrderCommandResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IPricingService _pricingService;
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUserService;
        
        public CreateOrderCommandHandler(
            IOrderRepository orderRepository, 
            IMapper mapper, 
            IPricingService pricingService, 
            IOrderService orderService,
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _pricingService = pricingService;
            _orderService = orderService;
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
            await _orderService.AddStatusHistoryAsync(order.Id, OrderStatus.Pending, "Order created");

            return new CreateOrderCommandResponse { Success = true, Message = $"Order created. ID: {order.OrderNumber}" };
        }

        
    }
}

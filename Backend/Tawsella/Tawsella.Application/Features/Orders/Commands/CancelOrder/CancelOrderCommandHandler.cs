using AutoMapper;
using FluentValidation;
using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, CancelOrderCommandResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUserService;
        
        public CancelOrderCommandHandler(
            IOrderRepository orderRepository,
            IMapper mapper, 
            IOrderService orderService,
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _orderService = orderService;
            _currentUserService = currentUserService;
        }

        public async Task<CancelOrderCommandResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(request.orderId))
                throw new ValidationException("Invalid order ID");

            var order = await _orderRepository.GetOrderWithCourierAsync(request.orderId, customerId, cancellationToken);

            if (order == null || order.Status != OrderStatus.Pending)
                return new CancelOrderCommandResponse { Message = "Order cannot be cancelled at this stage." };

            if (order.Courier != null) order.Courier.IsAvailable = true;

            order.Status = OrderStatus.Cancelled;
            order.CancellationReason = request.reason;

            await _orderService.AddStatusHistoryAsync(request.orderId, OrderStatus.Cancelled, $"Cancelled: {request.reason}");
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return new CancelOrderCommandResponse { Success = true, Message = "Order cancelled." };
        }
    }
}

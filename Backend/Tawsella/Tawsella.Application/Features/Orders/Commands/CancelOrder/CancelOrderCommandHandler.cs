using MediatR;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, BaseToReturnDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrentUserService _currentUserService;

        public CancelOrderCommandHandler(
            IOrderRepository orderRepository,
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _currentUserService = currentUserService;
        }

        public async Task<BaseToReturnDto> Handle(
            CancelOrderCommand request,
            CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();

            var order = await _orderRepository.GetOrderWithCourierAsync(
                request.OrderId,
                customerId,
                cancellationToken);

            if (order == null || order.Status != OrderStatus.Pending)
                return new BaseToReturnDto { Message = "Order cannot be cancelled at this stage." };

            if (order.Courier != null)
                order.Courier.IsAvailable = true;

            order.Status = OrderStatus.Cancelled;
            order.CancellationReason = request.Reason;

            await _orderRepository.AddStatusHistoryAsync(
                request.OrderId,
                OrderStatus.Cancelled,
                $"Cancelled: {request.Reason}");

            await _orderRepository.SaveChangesAsync(cancellationToken);

            return new BaseToReturnDto { Success = true, Message = "Order cancelled." };
        }
    }
}
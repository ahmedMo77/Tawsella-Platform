using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, UpdateOrderStatusResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUserService;
        
        public UpdateOrderStatusCommandHandler(
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

        public async Task<UpdateOrderStatusResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var order = await _orderRepository.GetOrderForCourierAsync(request.OrderId, courierId, cancellationToken);
            if (order == null) return new UpdateOrderStatusResponse { Success = false, Message = "Order not found." };

            order.Status = request.NewStatus;
            if (request.NewStatus == OrderStatus.Delivered) order.DeliveredAt = DateTime.UtcNow;

            await _orderService.AddStatusHistoryAsync(request.OrderId, request.NewStatus, request.Notes ?? $"Status updated to {request.NewStatus}");
            await _orderRepository.SaveChangesAsync(cancellationToken);
            return new UpdateOrderStatusResponse { Success = true, Message = "Status updated." };
        }
    }
}

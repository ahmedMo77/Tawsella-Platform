using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Enums;

namespace Tawsella.Application.Features.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, UpdateOrderStatusResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        
        public UpdateOrderStatusCommandHandler(
            IOrderRepository orderRepository,
            IMapper mapper, 
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<UpdateOrderStatusResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var order = await _orderRepository.GetOrderForCourierAsync(request.OrderId, courierId, cancellationToken);
            if (order == null) return new UpdateOrderStatusResponse { Success = false, Message = "Order not found." };

            order.Status = request.NewStatus;
            if (request.NewStatus == OrderStatus.Delivered) order.DeliveredAt = DateTime.UtcNow;

            await _orderRepository.AddStatusHistoryAsync(request.OrderId, request.NewStatus, request.Notes ?? $"Status updated to {request.NewStatus}");
            await _orderRepository.SaveChangesAsync(cancellationToken);
            return new UpdateOrderStatusResponse { Success = true, Message = "Status updated." };
        }
    }
}

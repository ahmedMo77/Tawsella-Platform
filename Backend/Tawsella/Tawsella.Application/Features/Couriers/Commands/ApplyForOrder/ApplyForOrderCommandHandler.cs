using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Couriers.Commands.ApplyForOrder
{
    public class ApplyForOrderCommandHandler 
        : IRequestHandler<ApplyForOrderCommand, ApplyForOrderCommandResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly ICurrentUserService _currentUserService;

        public ApplyForOrderCommandHandler(
            IOrderRepository orderRepository,
            ICourierRepository courierRepository,
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _courierRepository = courierRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ApplyForOrderCommandResponse> Handle(
            ApplyForOrderCommand request, 
            CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var courier = await _courierRepository.GetByIdAsync(courierId, cancellationToken);

            if (courier == null || !courier.IsApproved || !courier.IsOnline)
            {
                return new ApplyForOrderCommandResponse
                {
                    Message = "Courier is not eligible to apply."
                };
            }

            var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null || order.Status != OrderStatus.Pending)
            {
                return new ApplyForOrderCommandResponse
                {
                    Message = "Order is no longer available."
                };
            }

            var alreadyApplied = await _orderRepository.HasCourierAppliedAsync(request.OrderId, courierId, cancellationToken);

            if (alreadyApplied)
            {
                return new ApplyForOrderCommandResponse
                {
                    Message = "You have already applied for this order."
                };
            }

            var application = new OrderApplication
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = request.OrderId,
                CourierId = courierId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderApplicationStatus.Pending
            };

            await _orderRepository.AddOrderApplicationAsync(application, cancellationToken);

            return new ApplyForOrderCommandResponse
            {
                Success = true,
                Message = "Application sent to customer."
            };
        }
    }
}

using MediatR;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Couriers.Commands.PickupOrder
{
    public class PickupOrderCommandHandler 
        : IRequestHandler<PickupOrderCommand, BaseToReturnDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrentUserService _currentUserService;

        public PickupOrderCommandHandler(
            IOrderRepository orderRepository,
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _currentUserService = currentUserService;
        }

        public async Task<BaseToReturnDto> Handle(
            PickupOrderCommand request, 
            CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var order = await _orderRepository.GetOrderForCourierAsync(request.OrderId, courierId, cancellationToken);

            if (order == null)
            {
                return new BaseToReturnDto
                {
                    Success = false,
                    Message = "Order not found or not assigned to you."
                };
            }

            if (order.Status != OrderStatus.Accepted)
            {
                return new BaseToReturnDto
                {
                    Success = false,
                    Message = "Order must be in 'Accepted' status to pickup."
                };
            }

            order.Status = OrderStatus.PickedUp;
            order.MarkUpdated();

            await _orderRepository.UpdateAsync(order, cancellationToken);

            return new BaseToReturnDto
            {
                Success = true,
                Message = "Order picked up successfully!"
            };
        }
    }
}

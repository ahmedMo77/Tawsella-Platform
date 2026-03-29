using AutoMapper;
using FluentValidation;
using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Domain.Enums;
using Tawsella.Domain.Entities;
using Tawsella.Application.DTOs;
using Org.BouncyCastle.Asn1.Bsi;

namespace Tawsella.Application.Features.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, BaseToReturnDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        
        public CancelOrderCommandHandler(
            IOrderRepository orderRepository,
            IMapper mapper, 
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<BaseToReturnDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(request.OrderId))
                throw new ValidationException("Invalid order ID");

            Order order = await _orderRepository.GetOrderWithCourierAsync(request.OrderId, customerId, cancellationToken);

            if (order == null || order.Status != OrderStatus.Pending)
                return new BaseToReturnDto { Message = "Order cannot be cancelled at this stage." };

            if (order.Courier != null) order.Courier.IsAvailable = true;

            order.Status = OrderStatus.Cancelled;
            order.CancellationReason = request.Reason;

            await _orderRepository.AddStatusHistoryAsync(request.OrderId, OrderStatus.Cancelled, $"Cancelled: {request.Reason}");
            await _orderRepository.SaveChangesAsync(cancellationToken);

            return new BaseToReturnDto { Success = true, Message = "Order cancelled." };
        }
    }
}

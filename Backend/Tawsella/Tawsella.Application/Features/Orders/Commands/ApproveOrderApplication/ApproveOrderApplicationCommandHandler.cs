using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs;
using Tawsella.Domain.Enums;
using Tawsella.Application.Features.Notifications.Commands.SendNotification;

namespace Tawsella.Application.Features.Orders.Commands.ApproveOrderApplication
{

    public class ApproveOrderApplicationCommandHandler : IRequestHandler<ApproveOrderApplicationCommand, BaseToReturnDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        
        public ApproveOrderApplicationCommandHandler(
            IOrderRepository orderRepository,
            IMapper mapper, 
            IMediator mediator, 
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        public async Task<BaseToReturnDto> Handle(ApproveOrderApplicationCommand request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();
            
            var order = await _orderRepository.GetOrderForCustomerAsync(request.OrderId, customerId, cancellationToken);

            if (order == null || order.Status != OrderStatus.Pending)
                return new BaseToReturnDto { Success = false, Message = "Order not available for assignment." };

            var app = await _orderRepository.GetOrderApplicationWithCourierAsync(request.ApplicationId, request.OrderId, cancellationToken);

            if (app == null || !app.Courier.IsAvailable)
                return new BaseToReturnDto { Success = false, Message = "Courier is no longer available." };

            await _orderRepository.ApproveApplicationAsync(order, app, request.OrderId, request.ApplicationId, cancellationToken);
            await _orderRepository.AddStatusHistoryAsync(request.OrderId, OrderStatus.Accepted, $"Courier {app.CourierId} assigned", cancellationToken);
            
            // Send notification via MediatR
            await _mediator.Send(new SendNotificationCommand
            {
                UserId = app.CourierId,
                Title = "New Order!",
                Message = "You've been selected for a delivery",
                Type = NotificationType.ApplicationApproved
            }, cancellationToken);

            return new BaseToReturnDto { Success = true, Message = "Courier assigned successfully." };
        }
    }
}

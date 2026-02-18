using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Application.Features.Notifications.Commands.SendNotification;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Orders.Commands.ApproveOrderApplication
{

    public class ApproveOrderApplicationHandler : IRequestHandler<ApproveOrderApplicationCommand, ApproveOrderApplicationResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUserService;
        
        public ApproveOrderApplicationHandler(
            IOrderRepository orderRepository,
            IMapper mapper, 
            IMediator mediator, 
            IOrderService orderService,
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _mediator = mediator;
            _orderService = orderService;
            _currentUserService = currentUserService;
        }

        public async Task<ApproveOrderApplicationResponse> Handle(ApproveOrderApplicationCommand request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();
            
            var order = await _orderRepository.GetOrderForCustomerAsync(request.orderId, customerId, cancellationToken);

            if (order == null || order.Status != OrderStatus.Pending)
                return new ApproveOrderApplicationResponse { Success = false, Message = "Order not available for assignment." };

            var app = await _orderRepository.GetOrderApplicationWithCourierAsync(request.applicationId, request.orderId, cancellationToken);

            if (app == null || !app.Courier.IsAvailable)
                return new ApproveOrderApplicationResponse { Success = false, Message = "Courier is no longer available." };

            await _orderRepository.ApproveApplicationAsync(order, app, request.orderId, request.applicationId, cancellationToken);
            await _orderService.AddStatusHistoryAsync(request.orderId, OrderStatus.Accepted, $"Courier {app.CourierId} assigned");
            
            // Send notification via MediatR
            await _mediator.Send(new SendNotificationCommand
            {
                UserId = app.CourierId,
                Title = "New Order!",
                Message = "You've been selected for a delivery",
                Type = NotificationType.ApplicationApproved
            }, cancellationToken);

            return new ApproveOrderApplicationResponse { Success = true, Message = "Courier assigned successfully." };
        }
    }
}

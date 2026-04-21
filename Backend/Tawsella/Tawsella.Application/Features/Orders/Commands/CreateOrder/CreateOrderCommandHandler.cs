using AutoMapper;
using FluentValidation;
using MediatR;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.OrderDTOs;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Application.DTOs.PaymentDTOs;

namespace Tawsella.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand,CreateOrderCommandResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IPricingService _pricingService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPaymentService _paymentService;
        private readonly ICustomerRepository _customerRepository;
        
        public CreateOrderCommandHandler(
            IOrderRepository orderRepository, 
            IMapper mapper, 
            IPricingService pricingService, 
            ICurrentUserService currentUserService,
            IPaymentService paymentService,
            ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _pricingService = pricingService;
            _currentUserService = currentUserService;
            _paymentService = paymentService;
            _customerRepository = customerRepository;
        }
        public async Task<CreateOrderCommandResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();

            var customerExists = await _customerRepository.CustomerExistsAsync(customerId, cancellationToken);

            if (!customerExists) return new CreateOrderCommandResponse { Message = "Customer not found" };

            var CalculatePriceParameter = _mapper.Map<CalculatePriceDto>(request);
            var priceEstimate = _pricingService.CalculateOrderPrice(CalculatePriceParameter);

            var order = _mapper.Map<Order>(request);
            order.Id = Guid.NewGuid().ToString();
            order.OrderNumber = $"TW-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..5].ToUpper()}";
            order.CustomerId = customerId;
            order.Status = OrderStatus.Pending;
            order.Money.EstimatedPrice = priceEstimate.EstimatedPrice;
            order.Money.CourierEarnings = priceEstimate.CourierEarnings;
            order.Money.PlatformCommission = priceEstimate.PlatformCommission;

            if (request.PaymentMethod == PaymentMethod.Online)
            {
                order.PaymentStatus = PaymentStatus.Pending;
            }

            await _orderRepository.AddAsync(order, cancellationToken);
            await _orderRepository.AddStatusHistoryAsync(order.Id, OrderStatus.Pending, "Order created", cancellationToken);

            var response = new CreateOrderCommandResponse
            {
                Success = true,
                Message = $"Order created. ID: {order.OrderNumber}"
            };

            if (request.PaymentMethod == PaymentMethod.Online)
            {
                var customer = await _customerRepository.GetCustomerProfileAsync(customerId, cancellationToken);
                var customerEmail = customer?.User?.Email ?? "customer@tawsella.com";
                var customerPhone = customer?.User?.PhoneNumber ?? request.PickupContactPhone;
                var customerName = customer?.User?.FullName ?? request.PickupContactName;

                var paymentRequest = new PaymentRequestDto
                {
                    Order = order,
                    CustomerEmail = customerEmail,
                    CustomerPhone = customerPhone,
                    CustomerName = customerName
                };

                var paymentResult = await _paymentService.CreatePaymentAsync(paymentRequest);

                if (!paymentResult.Success)
                {
                    response.Message = $"Order created but payment initiation failed: {paymentResult.ErrorMessage}";
                    response.PaymentUrl = null;
                }
                else
                {
                    order.InvoiceId = paymentResult.InvoiceId;
                    await _orderRepository.UpdateAsync(order, cancellationToken);
                    
                    response.PaymentUrl = paymentResult.PaymentUrl;
                    response.Message = $"Order created. Please complete payment at the provided URL.";
                }
            }

            return response;
        }

        
    }
}

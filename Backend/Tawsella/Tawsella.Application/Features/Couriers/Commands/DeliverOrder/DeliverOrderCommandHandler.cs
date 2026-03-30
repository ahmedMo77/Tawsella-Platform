using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.OrderDTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Couriers.Commands.DeliverOrder
{
    public class DeliverOrderCommandHandler 
        : IRequestHandler<DeliverOrderCommand, DeliverOrderCommandResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPricingService _pricingService;
        private readonly ICurrentUserService _currentUserService;

        public DeliverOrderCommandHandler(
            IOrderRepository orderRepository,
            IPricingService pricingService,
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _pricingService = pricingService;
            _currentUserService = currentUserService;
        }

        public async Task<DeliverOrderCommandResponse> Handle(
            DeliverOrderCommand request, 
            CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var order = await _orderRepository.GetOrderForDeliveryAsync(request.OrderId, courierId, cancellationToken);

            if (order == null)
            {
                return new DeliverOrderCommandResponse
                {
                    Message = "Order not found or not assigned to you."
                };
            }

            if (order.Status != OrderStatus.PickedUp)
            {
                return new DeliverOrderCommandResponse
                {
                    Message = "Order not picked up yet"
                };
            }

            var priceDetails = _pricingService.CalculateOrderPrice(new CalculatePriceDto
            {
                PickupLatitude = order.Pickup.Location.Latitude,
                PickupLongitude = order.Pickup.Location.Longitude,
                DropoffLatitude = order.Dropoff.Location.Latitude,
                DropoffLongitude = order.Dropoff.Location.Longitude,
                PackageSize = order.Package.Size.ToString()
            });

            order.Status = OrderStatus.Delivered;
            order.DeliveredAt = DateTime.UtcNow;
            order.Money.FinalPrice = priceDetails.EstimatedPrice;
            order.Money.CourierEarnings = priceDetails.CourierEarnings;
            order.Money.PlatformCommission = priceDetails.PlatformCommission;
            order.MarkUpdated();

            WalletTransaction? transaction = null;

            if (order.Courier?.Wallet != null)
            {
                var wallet = order.Courier.Wallet;
                wallet.Balance += priceDetails.CourierEarnings;
                wallet.TotalEarnings += priceDetails.CourierEarnings;

                transaction = new WalletTransaction
                {
                    Id = Guid.NewGuid().ToString(),
                    WalletId = wallet.Id,
                    Amount = priceDetails.CourierEarnings,
                    BalanceAfter = wallet.Balance,
                    Type = TransactionType.OrderEarning,
                    Description = $"Earnings from Order #{order.OrderNumber}",
                    OrderId = order.Id,
                    CreatedAt = DateTime.UtcNow
                };
            }

            order.Courier.IsAvailable = true;
            await _orderRepository.CompleteDeliveryAsync(order, transaction, cancellationToken);

            return new DeliverOrderCommandResponse
            {
                Success = true,
                Message = "Delivered & Wallet Updated"
            };
        }
    }
}

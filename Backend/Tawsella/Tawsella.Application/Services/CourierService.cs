using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.DTOs.CourierDTOs;
using Tawsella.Domain.DTOs.OrderDTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Domain.Interfaces;

namespace Tawsella.Application.Services
{
    public class CourierService : ICourierService
    {
        private readonly IPricingService _pricingService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourierService(IUnitOfWork unitOfWork, IMapper mapper, IPricingService pricingService)
        {
            _pricingService = pricingService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pricingService = pricingService;
        }

        public async Task<CourierProfileDto> GetProfileAsync(string courierId)
        {
            var courier = await _unitOfWork.Couriers.Query
                .Include(c => c.User)
                .Include(c => c.Wallet)
                .FirstOrDefaultAsync(c => c.Id == courierId);

            return _mapper.Map<CourierProfileDto>(courier);
        }

        public async Task<BaseToReturnDto> UpdateProfileAsync(string courierId, UpdateCourierProfileDto model)
        {
            var courier = await _unitOfWork.Couriers.Query
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == courierId);

            if (courier == null) return new BaseToReturnDto { Success = false, Message = "Courier not found" };

            courier.User.FullName = model.FullName ?? courier.User.FullName;
            courier.User.PhoneNumber = model.PhoneNumber ?? courier.User.PhoneNumber;

            if (model.VehicleType.HasValue)
                courier.VehicleType = model.VehicleType.Value;

            courier.VehiclePlateNumber = model.VehiclePlateNumber ?? courier.VehiclePlateNumber;
            courier.MarkUpdated();

            await _unitOfWork.SaveChangesAsync();
            return new BaseToReturnDto { Success = true, Message = "Profile updated successfully" };
        }

        public async Task<BaseToReturnDto> UpdateOnlineStatusAsync(string courierId, bool isOnline)
        {
            var courier = await _unitOfWork.Couriers.Query.FirstOrDefaultAsync(c => c.Id == courierId);
            if (courier == null) return new BaseToReturnDto { Success = false };

            courier.IsOnline = isOnline;
            // لو قفل الأونلاين، نخليه غير متاح لاستقبال طلبات فوراً
            if (!isOnline) courier.IsAvailable = false;

            await _unitOfWork.SaveChangesAsync();
            return new BaseToReturnDto { Success = true };
        }

        public async Task UpdateLocationAsync(string courierId, UpdateLocationDto location)
        {
            var courier = await _unitOfWork.Couriers.Query.FirstOrDefaultAsync(c => c.Id == courierId);
            if (courier != null)
            {
                courier.CurrentLatitude = location.Latitude;
                courier.CurrentLongitude = location.Longitude;
                courier.LastLocationUpdate = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<CourierStatsDto> GetDashboardStatsAsync(string courierId)
        {
            var courier = await _unitOfWork.Couriers.Query
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == courierId);

            if (courier == null) return null;

            return new CourierStatsDto
            {
                AverageRating = courier.AverageRating,
                TotalReviews = courier.TotalReviews,
                CompletedDeliveries = courier.Orders.Count(o => o.Status == OrderStatus.Delivered)
            };
        }

        public async Task<List<CourierReviewItemDto>> GetRecentReviewsAsync(string courierId, int count = 5)
        {
            var reviews = await _unitOfWork.Reviews.Query
                .Where(r => r.CourierId == courierId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();

            return _mapper.Map<List<CourierReviewItemDto>>(reviews);
        }

        public async Task<List<OrderResponseDto>> GetAvailableOrdersAsync(string courierId, double radiusInKm = 10)
        {
            var courier = await _unitOfWork.Couriers.Query
                .FirstOrDefaultAsync(c => c.Id == courierId);

            if (courier == null || !courier.IsOnline || courier.CurrentLatitude == null)
                return new List<OrderResponseDto>();

            var availableOrders = await _unitOfWork.Orders.Query
                .Where(o => o.Status == OrderStatus.Pending && o.CourierId == null)
                .ToListAsync();

            // 3. نفلتر الطلبات اللي في نطاق الـ X كيلو متر اللي حددناهم
            // بنستخدم الـ PricingService اللي فيها معادلة الـ Haversine
            var nearbyOrders = availableOrders.Where(order =>
            {
                var distance = _pricingService.CalculateDistance(
                    courier.CurrentLatitude.Value,
                    courier.CurrentLongitude.Value,
                    order.PickupLatitude,
                    order.PickupLongitude
                );
                return distance <= radiusInKm;
            }).ToList();

            return _mapper.Map<List<OrderResponseDto>>(nearbyOrders);
        }

        public async Task<BaseToReturnDto> ApplyForOrderAsync(string courierId, string orderId)
        {
            var courier = await _unitOfWork.Couriers.Query.FirstOrDefaultAsync(c => c.Id == courierId);
            if (courier == null || !courier.IsApproved || !courier.IsOnline)
                return new BaseToReturnDto { Message = "Courier is not eligible to apply." };

            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null || order.Status != OrderStatus.Pending)
                return new BaseToReturnDto { Message = "Order is no longer available." };

            var alreadyApplied = await _unitOfWork.OrderApplications.Query
                .AnyAsync(a => a.OrderId == orderId && a.CourierId == courierId);
            if (alreadyApplied)
                return new BaseToReturnDto { Message = "You have already applied for this order." };

            var application = new OrderApplication
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = orderId,
                CourierId = courierId,
                CreatedAt = DateTime.UtcNow,
                Status = OrderApplicationStatus.Pending
            };

            await _unitOfWork.OrderApplications.AddAsync(application);
            await _unitOfWork.SaveChangesAsync();

            return new BaseToReturnDto { Success = true, Message = "Application sent to customer." };
        }

        public async Task<BaseToReturnDto> PickupOrderAsync(string courierId, string orderId)
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);

            if (order == null || order.CourierId != courierId)
                return new BaseToReturnDto { Success = false, Message = "Order not found or not assigned to you." };

            if (order.Status != OrderStatus.Accepted)
                return new BaseToReturnDto { Success = false, Message = "Order must be in 'Accepted' status to pickup." };

            order.Status = OrderStatus.PickedUp;
            order.MarkUpdated();

            await _unitOfWork.SaveChangesAsync();
            return new BaseToReturnDto { Success = true, Message = "Order picked up successfully!" };
        }

        public async Task<BaseToReturnDto> DeliverOrderAsync(string courierId, string orderId)
        {
            var order = await _unitOfWork.Orders.Query
                .Include(o => o.Courier).ThenInclude(c => c.Wallet)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CourierId == courierId);

            if (order == null || order.CourierId != courierId)
                return new BaseToReturnDto { Message = "Order not found or not assigned to you." };

            if (order.Status != OrderStatus.PickedUp)
                return new BaseToReturnDto { Message = "Order not picked up yet" };

            var priceDetails = _pricingService.CalculateOrderPrice(new CalculatePriceDto
            {
                PickupLatitude = order.PickupLatitude,
                PickupLongitude = order.PickupLongitude,
                DropoffLatitude = order.DropoffLatitude,
                DropoffLongitude = order.DropoffLongitude,
                PackageSize = order.PackageSize
            });

            order.Status = OrderStatus.Delivered;
            order.DeliveredAt = DateTime.UtcNow;
            order.FinalPrice = priceDetails.EstimatedPrice;
            order.CourierEarnings = priceDetails.CourierEarnings;
            order.PlatformCommission = priceDetails.PlatformCommission;
            order.MarkUpdated();

            if (order.Courier?.Wallet != null)
            {
                var wallet = order.Courier.Wallet;
                wallet.Balance += priceDetails.CourierEarnings;
                wallet.TotalEarnings += priceDetails.CourierEarnings;

                var transaction = new WalletTransaction
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
                await _unitOfWork.WalletTransactions.AddAsync(transaction);
            }

            order.Courier.IsAvailable = true;
            await _unitOfWork.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = OrderStatus.Delivered,
                CreatedAt = DateTime.UtcNow
            });

            await _unitOfWork.SaveChangesAsync();
            return new BaseToReturnDto { Success = true, Message = "Delivered & Wallet Updated" };
        }

        public async Task<OrderResponseDto?> GetActiveOrderAsync(string courierId)
        {
            // بنبحث عن أوردر مربوط بالمندوب ده وحالته مش منتهية
            var activeOrder = await _unitOfWork.Orders.Query
                //.Include(o => o.Items) // لو محتاج تفاصيل المنتجات
                .FirstOrDefaultAsync(o => o.CourierId == courierId &&
                                         (o.Status == OrderStatus.Accepted || o.Status == OrderStatus.PickedUp));

            if (activeOrder == null) return null;

            return _mapper.Map<OrderResponseDto>(activeOrder);
        }
    }
}

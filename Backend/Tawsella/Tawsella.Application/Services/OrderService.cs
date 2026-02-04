using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.DTOs.CourierDTOs;
using Tawsella.Domain.DTOs.OrderDTOs;
using Tawsella.Domain.DTOs.ReviewDTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Domain.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace Tawsella.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPricingService _pricingService;
        private readonly INotificationService _notificationService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IPricingService pricingService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pricingService = pricingService;
            _notificationService = notificationService;
        }

        public async Task<BaseToReturnDto> CreateOrderAsync(string customerId, CreateOrderDto dto)
        {
            if (string.IsNullOrEmpty(customerId) || dto == null)
                throw new ValidationException("Invalid order or customer");

            var customerExists = await _unitOfWork.Customers.Query
                .AnyAsync(c => c.Id == customerId);

            if (!customerExists) return new BaseToReturnDto { Message = "Customer not found" };

            var priceEstimate = _pricingService.CalculateOrderPrice(new CalculatePriceDto
            {
                PickupLatitude = dto.PickupLatitude,
                PickupLongitude = dto.PickupLongitude,
                DropoffLatitude = dto.DropoffLatitude,
                DropoffLongitude = dto.DropoffLongitude,
                PackageSize = dto.PackageSize,
                PackageWeight = dto.PackageWeight
            });

            var order = _mapper.Map<Order>(dto);
            order.Id = Guid.NewGuid().ToString();
            order.OrderNumber = $"TW-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..5].ToUpper()}";
            order.UserId = customerId;
            order.Status = OrderStatus.Pending;
            order.EstimatedPrice = priceEstimate.EstimatedPrice;
            order.CourierEarnings = priceEstimate.CourierEarnings;
            order.PlatformCommission = priceEstimate.PlatformCommission;

            await _unitOfWork.Orders.AddAsync(order);
            await AddStatusHistoryAsync(order.Id, OrderStatus.Pending, "Order created");
            await _unitOfWork.SaveChangesAsync();

            return new BaseToReturnDto { Success = true, Message = $"Order created. ID: {order.OrderNumber}" };
        }

        public async Task<BaseToReturnDto> CancelOrderAsync(string customerId, string orderId, string reason)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(orderId))
                throw new ValidationException("Invalid order ID or user ID");

            var order = await _unitOfWork.Orders.Query
                .Include(o => o.Courier)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == customerId);

            if (order == null || order.Status != OrderStatus.Pending)
                return new BaseToReturnDto { Message = "Order cannot be cancelled at this stage." };

            if (order.Courier != null) order.Courier.IsAvailable = true;

            order.Status = OrderStatus.Cancelled;
            order.CancellationReason = reason;

            await AddStatusHistoryAsync(orderId, OrderStatus.Cancelled, $"Cancelled: {reason}");
            await _unitOfWork.SaveChangesAsync();

            return new BaseToReturnDto { Success = true, Message = "Order cancelled." };
        }

        public async Task<OrderResponseDto> GetOrderDetailsAsync(string orderId, string userId)
        {
            if (string.IsNullOrEmpty(orderId) || string.IsNullOrEmpty(userId))
                throw new ValidationException("Invalid order ID or user ID");

            var order = await _unitOfWork.Orders.Query
                .Where(o => o.Id == orderId && (o.UserId == userId || o.CourierId == userId))
                .ProjectTo<OrderResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return order ?? throw new KeyNotFoundException("Order not found.");
        }

        public async Task<BaseToReturnDto> ApproveOrderApplicationAsync(string customerId, string orderId, string applicationId)
        {
            var order = await _unitOfWork.Orders.Query
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == customerId);

            if (order == null || order.Status != OrderStatus.Pending)
                return new BaseToReturnDto { Success = false, Message = "Order not available for assignment." };

            var app = await _unitOfWork.OrderApplications.Query
                .Include(a => a.Courier)
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.OrderId == orderId);

            if (app == null || !app.Courier.IsAvailable)
                return new BaseToReturnDto { Success = false, Message = "Courier is no longer available." };

            app.Status = OrderApplicationStatus.Approved;
            order.CourierId = app.CourierId;
            order.Status = OrderStatus.Accepted;
            order.AcceptedAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            app.Courier.IsAvailable = false;

            _unitOfWork.Orders.Update(order);
            _unitOfWork.Couriers.Update(app.Courier);

            // Reject other applications
            await _unitOfWork.OrderApplications.Query
                .Where(a => a.OrderId == orderId && a.Id != applicationId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.Status, OrderApplicationStatus.Rejected)
                    .SetProperty(a => a.RejectedReason, "Another courier selected"));

            await AddStatusHistoryAsync(orderId, OrderStatus.Accepted, $"Courier {app.CourierId} assigned");
            await _unitOfWork.SaveChangesAsync();
            await _notificationService.SendNotificationAsync(app.CourierId, "New Order!", "You've been selected for a delivery", NotificationType.ApplicationApproved);

            return new BaseToReturnDto { Success = true, Message = "Courier assigned successfully." };
        }

        public async Task<PaginatedResponseDto<OrderResponseDto>> GetOrdersHistoryAsync(string userId, OrderStatus? status, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ValidationException("Invalid user ID");

            var query = _unitOfWork.Orders.Query.Where(o => o.UserId == userId || o.CourierId == userId);
            if (status.HasValue) query = query.Where(o => o.Status == status.Value);

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(o => o.CreatedAt)
                                   .Skip((page - 1) * pageSize).Take(pageSize)
                                   .ProjectTo<OrderResponseDto>(_mapper.ConfigurationProvider)
                                   .ToListAsync();

            return new PaginatedResponseDto<OrderResponseDto> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
        }

        public async Task<List<OrderApplicationWithCourierDto>> GetOrderApplicationsAsync(string customerId, string orderId)
        {
            // 1. التأكد من ملكية الطلب
            var orderExists = await _unitOfWork.Orders.Query
                .AnyAsync(o => o.Id == orderId && o.UserId == customerId);

            if (!orderExists)
                throw new KeyNotFoundException("Order not found or access denied");

            return await _unitOfWork.OrderApplications.Query
                .Where(a => a.OrderId == orderId && a.Status == OrderApplicationStatus.Pending)
                .ProjectTo<OrderApplicationWithCourierDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<BaseToReturnDto> SubmitReviewAsync(string customerId, string orderId, CreateReviewDto dto)
        {
            var order = await _unitOfWork.Orders.Query.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == customerId);
            if (order == null || order.Status != OrderStatus.Delivered)
                return new BaseToReturnDto { Message = "Review only allowed for delivered orders." };

            var review = _mapper.Map<Review>(dto);
            review.Id = Guid.NewGuid().ToString();
            review.OrderId = orderId;
            review.UserId = customerId;
            review.CourierId = order.CourierId;

            var courier = await _unitOfWork.Couriers.Query
                .FirstOrDefaultAsync(c => c.Id == order.CourierId);

            if (courier != null)
            {
                double totalScore = (courier.AverageRating * courier.TotalReviews) + dto.Rating;
                courier.TotalReviews++;
                courier.AverageRating = Math.Round(totalScore / courier.TotalReviews, 2);

                _unitOfWork.Couriers.Update(courier);
            }

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();
            return new BaseToReturnDto { Success = true, Message = "Review submitted." };
        }

        public async Task<ReviewDto> GetOrderReviewAsync(string customerId, string orderId)
        {
            if (string.IsNullOrEmpty(customerId) || string.IsNullOrEmpty(orderId))
                throw new ValidationException("Invalid order ID or user ID");

            return await _unitOfWork.Reviews.Query
                .Where(r => r.OrderId == orderId && r.UserId == customerId)
                .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<BaseToReturnDto> UpdateOrderStatusAsync(string orderId, string courierId, OrderStatus newStatus, string? notes)
        {
            var order = await _unitOfWork.Orders.Query.FirstOrDefaultAsync(o => o.Id == orderId && o.CourierId == courierId);
            if (order == null) return new BaseToReturnDto { Success = false, Message = "Order not found." };

            order.Status = newStatus;
            if (newStatus == OrderStatus.Delivered) order.DeliveredAt = DateTime.UtcNow;

            await AddStatusHistoryAsync(orderId, newStatus, notes ?? $"Status updated to {newStatus}");
            await _unitOfWork.SaveChangesAsync();
            return new BaseToReturnDto { Success = true, Message = "Status updated." };
        }

        private async Task AddStatusHistoryAsync(string orderId, OrderStatus status, string notes)
        {
            await _unitOfWork.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = orderId,
                Status = status,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}

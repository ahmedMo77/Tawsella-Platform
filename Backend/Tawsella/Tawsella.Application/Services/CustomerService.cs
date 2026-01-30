using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.CustomerDTOs;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Infrastructure.DbContext;

namespace Tawsella.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CustomerService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<CustomerProfileDto> GetProfile(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            var profile = await _context.Customers
                .Where(c => c.Id == customerId)
                .ProjectTo<CustomerProfileDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (profile == null)
                throw new KeyNotFoundException($"Customer with ID {customerId} not found");

            return profile;
        }

        public async Task<BaseToReturnDto> UpdateCustomerProfile(string customerId, UpdateProfileDto dto)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
            {
                return new BaseToReturnDto
                {
                    Message = "Customer not found",
                    Success = false
                };
            }

            _mapper.Map(dto, customer);
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new BaseToReturnDto
            {
                Message = "Profile updated successfully",
                Success = true
            };
        }

        public async Task<CustomerStatisticsDto> GetCustomerStatistics(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            var statistics = await _context.Orders
            .Where(o => o.UserId == customerId)
             .GroupBy(o => o.UserId)
            .Select(g => new CustomerStatisticsDto
            {
                TotalOrders = g.Count(),
                CompletedOrders = g.Count(o => o.Status == OrderStatus.Delivered),
                CancelledOrders = g.Count(o => o.Status == OrderStatus.Cancelled),
                TotalSpent = g
              .Where(o => o.Status == OrderStatus.Delivered)
              .Sum(o => o.FinalPrice ?? 0),
                AverageOrderValue = g
              .Where(o => o.Status == OrderStatus.Delivered)
              .Any()
              ? g.Where(o => o.Status == OrderStatus.Delivered)
                   .Average(o => o.FinalPrice ?? 0)
              : 0
            })
             .FirstOrDefaultAsync();
            if (statistics == null)
                throw new KeyNotFoundException($"Customer with ID {customerId} not found");

            return statistics;
        }

        public async Task<PriceEstimateDto> CalculateOrderPrice(CalculatePriceDto dto)
        {
            if (dto == null)
                throw new ValidationException("Invalid price calculation data");

            // Calculate distance using Haversine formula
            var distance = CalculateDistance(
                dto.PickupLatitude,
                dto.PickupLongitude,
                dto.DropoffLatitude,
                dto.DropoffLongitude
            );

            // Base price (30 EGP)
            decimal basePrice = 30.00m;

            // Distance fee (3 EGP per km)
            decimal distanceFee = (decimal)distance * 3.00m;

            // Package size multiplier
            decimal sizeMultiplier = dto.PackageSize?.ToLower() switch
            {
                "small" => 1.0m,
                "medium" => 1.3m,
                "large" => 1.6m,
                _ => 1.0m
            };

            // Time multiplier (peak hours: 12-14, 18-21)
            var hour = DateTime.UtcNow.Hour;
            decimal timeMultiplier = (hour >= 12 && hour <= 14) || (hour >= 18 && hour <= 21)
                ? 1.2m
                : 1.0m;

            // Calculate total
            decimal totalPrice = (basePrice + distanceFee) * sizeMultiplier * timeMultiplier;
            decimal courierEarnings = totalPrice * 0.85m; // 85% to courier
            decimal platformCommission = totalPrice * 0.15m; // 15% platform

            // Estimate delivery time (30 km/h average speed)
            var timeInMinutes = (int)Math.Ceiling((distance / 30.0) * 60);

            return new PriceEstimateDto
            {
                EstimatedPrice = Math.Round(totalPrice, 2),
                Distance = Math.Round(distance, 1),
                BasePrice = basePrice,
                DistanceFee = Math.Round(distanceFee, 2),
                SizeMultiplier = sizeMultiplier,
                TimeMultiplier = timeMultiplier,
                CourierEarnings = Math.Round(courierEarnings, 2),
                PlatformCommission = Math.Round(platformCommission, 2),
                EstimatedDeliveryTime = $"{timeInMinutes}-{timeInMinutes + 15} minutes"
            };
        }

        public async Task<BaseToReturnDto> CreateOrder(string customerId, CreateOrderDto dto)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            if (dto == null)
                throw new ValidationException("Invalid order data");

            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == customerId);

            if (!customerExists)
            {
                return new BaseToReturnDto
                {
                    Message = "Customer not found",
                    Success = false
                };
            }

            // Calculate price
            var priceEstimate = await CalculateOrderPrice(new CalculatePriceDto
            {
                PickupLatitude = dto.PickupLatitude,
                PickupLongitude = dto.PickupLongitude,
                DropoffLatitude = dto.DropoffLatitude,
                DropoffLongitude = dto.DropoffLongitude,
                PackageSize = dto.PackageSize,
                PackageWeight = dto.PackageWeight
            });
            var orderNumber = $"TW-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}";

            var order = _mapper.Map<Order>(dto);
            order.OrderNumber = orderNumber;
            order.UserId = customerId;
            order.EstimatedPrice = priceEstimate.EstimatedPrice;
            order.CourierEarnings = priceEstimate.CourierEarnings;
            order.PlatformCommission = priceEstimate.PlatformCommission;


            await _context.Orders.AddAsync(order);

            var statusHistory = new OrderStatusHistory
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = order.Id,
                Status = OrderStatus.Pending,
                Notes = "Order created",
                CreatedAt = DateTime.UtcNow
            };
            await _context.OrderStatusHistories.AddAsync(statusHistory);

            await _context.SaveChangesAsync();

            return new BaseToReturnDto
            {
                Message = $"Order created successfully. Order number: {orderNumber}",
                Success = true
            };
        }

        public async Task<PaginatedResponseDto<OrderResponseDto>> GetCustomerOrders(
            string customerId,
            OrderStatus? status,
            int page,
            int pageSize)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            if (page <= 0)
                throw new ValidationException("Page number must be greater than 0");

            if (pageSize <= 0 || pageSize > 100)
                throw new ValidationException("Page size must be between 1 and 100");

            var query = _context.Orders
                .Where(o => o.UserId == customerId)
                .AsNoTracking();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<OrderResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PaginatedResponseDto<OrderResponseDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = orders,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<OrderResponseDto> GetOrderDetails(string customerId, string orderId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            if (string.IsNullOrEmpty(orderId))
                throw new ValidationException("Invalid order ID format");

            var order = await _context.Orders
                .Where(o => o.Id == orderId && o.UserId == customerId)
                .ProjectTo<OrderResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found or you don't have permission to view it");

            return order;
        }

        public async Task<BaseToReturnDto> CancelOrder(string customerId, string orderId, string reason)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            if (string.IsNullOrEmpty(orderId))
                throw new ValidationException("Invalid order ID format");

            var order = await _context.Orders
                .Include(o => o.Courier)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == customerId);

            if (order == null)
            {
                return new BaseToReturnDto
                {
                    Message = "Order not found or you don't have permission to cancel it",
                    Success = false
                };
            }

            if (order.Status != OrderStatus.Pending)
            {
                return new BaseToReturnDto
                {
                    Message = "Only pending orders can be cancelled",
                    Success = false
                };
            }

            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;
            order.CancellationReason = reason;
            order.UpdatedAt = DateTime.UtcNow;


            var statusHistory = await _context.OrderStatusHistories.Where(o => o.OrderId == orderId).FirstOrDefaultAsync();
            statusHistory.Status = OrderStatus.Cancelled;
            statusHistory.Notes = $"Cancelled by customer: {reason}";
            statusHistory.UpdatedAt = DateTime.UtcNow;
    

            if (order.CourierId != null && order.Courier != null)
            {
                order.Courier.IsAvailable = true;
            }

            await _context.SaveChangesAsync();


            return new BaseToReturnDto
            {
                Message = "Order cancelled successfully",
                Success = true
            };
        }

        public async Task<BaseToReturnDto> SubmitReview(string customerId, string orderId, CreateReviewDto dto)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            if (string.IsNullOrEmpty(orderId))
                throw new ValidationException("Invalid order ID format");

            if (dto == null || dto.Rating < 1 || dto.Rating > 5)
                throw new ValidationException("Rating must be between 1 and 5");

            var order = await _context.Orders
                .Include(o => o.Review)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == customerId);

            if (order == null)
            {
                return new BaseToReturnDto
                {
                    Message = "Order not found or you don't have permission to review it",
                    Success = false
                };
            }

            if (order.Status != OrderStatus.Delivered)
            {
                return new BaseToReturnDto
                {
                    Message = "Can only review delivered orders",
                    Success = false
                };
            }

            if (order.Review != null)
            {
                return new BaseToReturnDto
                {
                    Message = "Order has already been reviewed",
                    Success = false
                };
            }

            if (string.IsNullOrEmpty(order.CourierId))
            {
                return new BaseToReturnDto
                {
                    Message = "Cannot review order without assigned courier",
                    Success = false
                };
            }

            // Create review
            var review = new Review
            {
                Id = Guid.NewGuid().ToString(),
                OrderId = order.Id,
                UserId = customerId,
                CourierId = order.CourierId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return new BaseToReturnDto
            {
                Message = "Review submitted successfully",
                Success = true
            };
        }

        public async Task<ReviewDto> GetOrderReview(string customerId, string orderId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            if (string.IsNullOrEmpty(orderId))
                throw new ValidationException("Invalid order ID format");

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == customerId);

            if (order == null)
                throw new KeyNotFoundException("Order not found or you don't have permission to view it");

            var review = await _context.Reviews
                .Where(r => r.OrderId == orderId)
                .ProjectTo<ReviewDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return review; 
        }
        public async Task<PaginatedResponseDto<NotificationDto>> GetCustomerNotifications(
            string customerId,
            bool unreadOnly,
            int page,
            int pageSize)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            if (page <= 0)
                throw new ValidationException("Page number must be greater than 0");

            if (pageSize <= 0 || pageSize > 100)
                throw new ValidationException("Page size must be between 1 and 100");

            var query = _context.Notifications
                .Where(n => n.UserId == customerId)
                .AsNoTracking();

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            var totalCount = await query.CountAsync();
            var unreadCount = await _context.Notifications
                .CountAsync(n => n.UserId == customerId && !n.IsRead);

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PaginatedResponseDto<NotificationDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = notifications,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                UnreadCount = unreadCount
            };
        }

        public async Task<BaseToReturnDto> MarkNotificationAsRead(string customerId, string notificationId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            if (string.IsNullOrEmpty(notificationId))
                throw new ValidationException("Invalid notification ID format");

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == customerId);

            if (notification == null)
            {
                return new BaseToReturnDto
                {
                    Message = "Notification not found",
                    Success = false
                };
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return new BaseToReturnDto
            {
                Message = "Notification marked as read",
                Success = true
            };
        }

        public async Task<BaseToReturnDto> MarkAllNotificationsAsRead(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == customerId && !n.IsRead)
                .ToListAsync();

            if (unreadNotifications.Any())
            {
                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            return new BaseToReturnDto
            {
                Message = $"{unreadNotifications.Count} notification(s) marked as read",
                Success = true
            };
        }

        private double CalculateDistance(decimal lat1, decimal lng1, decimal lat2, decimal lng2)
        {
            const double R = 6371; // Earth's radius in km

            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lng2 - lng1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; // Distance in km
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
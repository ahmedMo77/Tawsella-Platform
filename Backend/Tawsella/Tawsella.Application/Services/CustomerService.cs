using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Application.DTOs.CustomerDTOs;
using Tawsella.Application.Interfaces;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.DTOs.CourierDTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Domain.Interfaces;

namespace Tawsella.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPricingService _pricingService;
        private readonly INotificationService _notificationService;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, IPricingService pricingService, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _pricingService = pricingService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CustomerProfileDto> GetProfile(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ValidationException("Invalid customer ID format");

            var profile = await _unitOfWork.Customers.Query
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

            var customer = await _unitOfWork.Customers.Query
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
            _unitOfWork.Customers.Update(customer);
            await _unitOfWork.SaveChangesAsync();

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

            var statistics = await _unitOfWork.Orders.Query
                .Where(o => o.UserId == customerId)
                .GroupBy(o => o.UserId)
                .Select(g => new CustomerStatisticsDto
                {
                    TotalOrders = g.Count(),
                    CompletedOrders = g.Count(o => o.Status == OrderStatus.Delivered),
                    CancelledOrders = g.Count(o => o.Status == OrderStatus.Cancelled),
                    TotalSpent = g.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.FinalPrice ?? 0),
                    AverageOrderValue = g.Where(o => o.Status == OrderStatus.Delivered).Any()
                        ? g.Where(o => o.Status == OrderStatus.Delivered).Average(o => o.FinalPrice ?? 0)
                        : 0
                })
                .FirstOrDefaultAsync();

            return statistics ?? new CustomerStatisticsDto
            {
                TotalOrders = 0,
                CompletedOrders = 0,
                CancelledOrders = 0,
                TotalSpent = 0,
                AverageOrderValue = 0
            };
        }

        public async Task<CourierPublicProfileDto> GetCourierPublicProfileAsync(string courierId)
        {
            var courier = await _unitOfWork.Couriers.Query
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == courierId);
            if (courier == null)
                throw new KeyNotFoundException("Courier not found.");

            var reviews = await _unitOfWork.Reviews.Query
                .Where(r => r.CourierId == courierId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(50)
                .Select(r => new CourierReviewItemDto
                {
                    Rating = r.Rating,
                    Comment = r.Comment ?? "",
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            var ratingList = await _unitOfWork.Reviews.Query
                .Where(r => r.CourierId == courierId)
                .Select(r => r.Rating)
                .ToListAsync();
            var averageRating = ratingList.Any() ? Math.Round(ratingList.Average(), 2) : 0;
            var completedDeliveries = await _unitOfWork.Orders.Query
                .CountAsync(o => o.CourierId == courierId && o.Status == OrderStatus.Delivered);

            return new CourierPublicProfileDto
            {
                Id = courier.Id,
                FullName = courier.User?.FullName ?? "",
                VehicleType = courier.VehicleType,
                AverageRating = averageRating,
                TotalReviews = ratingList.Count,
                CompletedDeliveries = completedDeliveries,
                Reviews = reviews
            };
        }
    }
}
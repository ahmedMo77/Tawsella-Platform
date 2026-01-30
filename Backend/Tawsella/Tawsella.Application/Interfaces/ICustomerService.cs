using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.CustomerDTOs;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerProfileDto> GetProfile(string CustomerId);
        Task<BaseToReturnDto> UpdateCustomerProfile(string CustomerId, UpdateProfileDto dto);
        Task<CustomerStatisticsDto> GetCustomerStatistics(string CustomerId);
        Task<PriceEstimateDto> CalculateOrderPrice(CalculatePriceDto dto);
        Task<BaseToReturnDto> CreateOrder(string CustomerId, CreateOrderDto dto);
        Task<PaginatedResponseDto<OrderResponseDto>> GetCustomerOrders(string CustomerId, OrderStatus? status, int page, int pageSize);
        Task<OrderResponseDto> GetOrderDetails(string CustomerId, string orderId);
        Task<BaseToReturnDto> CancelOrder(string customerId, string orderId, string reason);
        Task<BaseToReturnDto> SubmitReview(string customerId, string orderId, CreateReviewDto dto);
        Task<ReviewDto> GetOrderReview(string customerId, string orderId);
        Task<PaginatedResponseDto<NotificationDto>> GetCustomerNotifications(string customerId, bool unreadOnly, int page, int pageSize);
        Task<BaseToReturnDto> MarkNotificationAsRead(string customerId, string notificationId);
        Task<BaseToReturnDto> MarkAllNotificationsAsRead(string customerId);





    }
}
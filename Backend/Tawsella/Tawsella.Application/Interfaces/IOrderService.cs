using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.DTOs.OrderDTOs;
using Tawsella.Domain.DTOs.ReviewDTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Interfaces
{
    public interface IOrderService
    {
        Task<BaseToReturnDto> CreateOrderAsync(string customerId, CreateOrderDto dto);
        Task<BaseToReturnDto> CancelOrderAsync(string customerId, string orderId, string reason);
        Task<OrderResponseDto> GetOrderDetailsAsync(string orderId, string userId);
        Task<PaginatedResponseDto<OrderResponseDto>> GetOrdersHistoryAsync(string userId, OrderStatus? status, int page, int pageSize);
        Task<List<OrderApplicationWithCourierDto>> GetOrderApplicationsAsync(string customerId, string orderId);
        Task<BaseToReturnDto> ApproveOrderApplicationAsync(string customerId, string orderId, string applicationId);

        Task<BaseToReturnDto> SubmitReviewAsync(string customerId, string orderId, CreateReviewDto dto);
        Task<ReviewDto> GetOrderReviewAsync(string customerId, string orderId);

        Task<BaseToReturnDto> UpdateOrderStatusAsync(string orderId, string courierId, OrderStatus newStatus, string? notes);
    }
}

using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.DTOs.CourierDTOs;
using Tawsella.Domain.DTOs.OrderDTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Interfaces
{
    public interface ICourierService
    {
        Task<CourierProfileDto> GetProfileAsync(string courierId);
        Task<BaseToReturnDto> UpdateProfileAsync(string courierId, UpdateCourierProfileDto model);
        Task<BaseToReturnDto> UpdateOnlineStatusAsync(string courierId, bool isOnline);

        Task UpdateLocationAsync(string courierId, UpdateLocationDto location);

        Task<CourierStatsDto> GetDashboardStatsAsync(string courierId);
        Task<List<CourierReviewItemDto>> GetRecentReviewsAsync(string courierId, int count = 5);

        Task<List<OrderResponseDto>> GetAvailableOrdersAsync(string courierId, double radiusInKm = 10);  // maybe overload with location or city parameters instead of courierId
        Task<BaseToReturnDto> ApplyForOrderAsync(string courierId, string orderId);

        Task<BaseToReturnDto> PickupOrderAsync(string courierId, string orderId);
        Task<BaseToReturnDto> DeliverOrderAsync(string courierId, string orderId);

        Task<OrderResponseDto?> GetActiveOrderAsync(string courierId);


        //Task<PaginatedResponseDto<AvailableOrderDto>> GetAvailableOrdersAsync(string courierId, int page, int pageSize);
        //Task<BaseToReturnDto> ApplyToOrderAsync(string courierId, string orderId);
        //Task<PaginatedResponseDto<OrderApplicationDto>> GetMyApplicationsAsync(string courierId, OrderApplicationStatus? status, int page, int pageSize);
        //Task<BaseToReturnDto> UpdateProfileAsync(string courierId, UpdateCourierProfileDto dto);
        //Task<PaginatedResponseDto<CourierReviewItemDto>> GetMyReviewsAsync(string courierId, int page, int pageSize);
        //Task<double> GetMyRatingAsync(string courierId);
        //Task<BaseToReturnDto> MarkPickedUpAsync(string courierId, string orderId);
        //Task<BaseToReturnDto> MarkDeliveredAsync(string courierId, string orderId);
    }
}

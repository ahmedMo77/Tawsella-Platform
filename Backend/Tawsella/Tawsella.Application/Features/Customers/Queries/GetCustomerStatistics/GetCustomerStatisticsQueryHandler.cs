using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs.CustomerDTOs;
using Tawsella.Application.Enums;

namespace Tawsella.Application.Features.Customers.Queries.GetCustomerStatistics
{
    public class GetCustomerStatisticsQueryHandler 
        : IRequestHandler<GetCustomerStatisticsQuery, GetCustomerStatisticsQueryResponse>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetCustomerStatisticsQueryHandler(
            ICustomerRepository customerRepository,
            ICurrentUserService currentUserService)
        {
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetCustomerStatisticsQueryResponse> Handle(
            GetCustomerStatisticsQuery request, 
            CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();

            var (totalOrders, totalSpent) = await _customerRepository.GetCustomerStatisticsAsync(
                customerId, 
                cancellationToken);

            var statistics = new CustomerStatisticsDto
            {
                TotalOrders = totalOrders,
                CompletedOrders = totalOrders, // Repository returns count of all orders
                CancelledOrders = 0, // Can be enhanced later if needed
                TotalSpent = totalSpent,
                AverageOrderValue = totalOrders > 0 ? totalSpent / totalOrders : 0
            };

            return new GetCustomerStatisticsQueryResponse
            {
                Statistics = statistics
            };
        }
    }
}

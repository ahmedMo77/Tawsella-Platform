using AutoMapper;
using MediatR;
using System;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.DTOs.OrderDTOs;

namespace Tawsella.Application.Features.Orders.Queries.GetOrdersHistory
{
    public class GetOrdersHistoryQueryHandler : IRequestHandler<GetOrdersHistoryQuery, GetOrdersHistoryQueryResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        
        public GetOrdersHistoryQueryHandler(
            IOrderRepository orderRepository, 
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<GetOrdersHistoryQueryResponse> Handle(GetOrdersHistoryQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();

            var (orders, totalCount) = await _orderRepository.GetOrdersHistoryAsync(
                userId, 
                request.status, 
                request.page, 
                request.pageSize, 
                cancellationToken);

            var items = _mapper.Map<List<OrderResponseDto>>(orders);

            PaginatedResponseDto<OrderResponseDto> response = new PaginatedResponseDto<OrderResponseDto>()
            {
                Items = items,
                TotalCount = totalCount,
                Page = request.page,
                PageSize = request.pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.pageSize)
            };

            return new GetOrdersHistoryQueryResponse { paginatedResponse = response };

        }
    }

}

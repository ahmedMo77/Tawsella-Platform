using AutoMapper;
using MediatR;
using System;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.OrderDTOs;

namespace Tawsella.Application.Features.Orders.Queries.GetOrdersHistory
{
    public class GetOrdersHistoryQueryHandler
        : IRequestHandler<GetOrdersHistoryQuery, GetOrdersHistoryQueryResponse>
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

        public async Task<GetOrdersHistoryQueryResponse> Handle(
            GetOrdersHistoryQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();

            var (orders, totalCount) = await _orderRepository.GetOrdersHistoryAsync(
                userId,
                request.Status,
                request.Page,
                request.PageSize,
                cancellationToken);

            return new GetOrdersHistoryQueryResponse
            {
                Items = _mapper.Map<List<OrderResponseDto>>(orders),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }
}

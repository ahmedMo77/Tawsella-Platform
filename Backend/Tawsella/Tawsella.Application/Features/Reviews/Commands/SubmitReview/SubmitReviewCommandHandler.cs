using AutoMapper;
using MediatR;
using System;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Entities;
using Tawsella.Application.Enums;

namespace Tawsella.Application.Features.Reviews.Commands.SubmitReview
{
    public class SubmitReviewCommandHandler : IRequestHandler<SubmitReviewCommand, SubmitReviewCommandResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public SubmitReviewCommandHandler(
            IOrderRepository orderRepository,
            IReviewRepository reviewRepository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        public async Task<SubmitReviewCommandResponse> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();
            
            var order = await _orderRepository.GetOrderForCustomerAsync(request.orderId, customerId, cancellationToken);
            if (order == null || order.Status != OrderStatus.Delivered)
                return new SubmitReviewCommandResponse { Message = "Review only allowed for delivered orders." };

            var review = _mapper.Map<Review>(request.dto);
            review.Id = Guid.NewGuid().ToString();
            review.OrderId = request.orderId;
            review.UserId = customerId;
            review.CourierId = order.CourierId;

            // Use repository method that handles both review creation and courier stats update atomically
            await _reviewRepository.AddReviewAndUpdateCourierStatsAsync(
                review, 
                order.CourierId, 
                request.dto.Rating, 
                cancellationToken);

            return new SubmitReviewCommandResponse { Success = true, Message = "Review submitted." };
        }
    }
}

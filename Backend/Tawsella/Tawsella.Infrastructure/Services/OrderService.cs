using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.DTOs.OrderDTOs;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;
using Tawsella.Domain.Contracts;
using Tawsella.Application.Interfaces;

namespace Tawsella.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task AddStatusHistoryAsync(string orderId, OrderStatus status, string notes)
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

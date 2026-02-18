using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Domain.DTOs;
using Tawsella.Domain.DTOs.OrderDTOs;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Interfaces
{
    public interface IOrderService
    {
        Task AddStatusHistoryAsync(string orderId, OrderStatus status, string notes);
    }
}

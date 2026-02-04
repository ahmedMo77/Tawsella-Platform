using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Domain.DTOs.OrderDTOs;

namespace Tawsella.Domain.DTOs.CourierDTOs
{
    public class CourierHomeDto
    {
        public CourierProfileDto Profile { get; set; }
        public CourierStatsDto Stats { get; set; }
        public List<OrderResponseDto> ActiveOrders { get; set; }
    }
}

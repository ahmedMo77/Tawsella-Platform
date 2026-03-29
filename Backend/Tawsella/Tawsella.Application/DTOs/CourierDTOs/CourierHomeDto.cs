using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.CourierDTOs;
using Tawsella.Application.DTOs.OrderDTOs;

namespace Tawsella.Application.DTOs.CourierDTOs
{
    public class CourierHomeDto
    {
        public CourierProfileDto Profile { get; set; }
        public CourierStatsDto Stats { get; set; }
        public List<OrderResponseDto> ActiveOrders { get; set; }
    }
}

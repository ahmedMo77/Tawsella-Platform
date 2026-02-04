using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Domain.DTOs.CourierDTOs
{
    public class CourierStatsDto
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int CompletedDeliveries { get; set; }
    }
}

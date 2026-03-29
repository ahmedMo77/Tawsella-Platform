using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tawsella.Application.DTOs.CourierDTOs
{
    public class CourierReviewItemDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}

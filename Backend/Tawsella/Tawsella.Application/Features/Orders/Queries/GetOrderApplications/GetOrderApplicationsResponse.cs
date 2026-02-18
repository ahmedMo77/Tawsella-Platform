using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.CourierDTOs;

namespace Tawsella.Application.Features.Orders.Queries.GetOrderApplications
{
    public class GetOrderApplicationsResponse
    {

        public string ApplicationId { get; set; }
        public string OrderId { get; set; }
        public string CourierId { get; set; }
        public DateTime AppliedAt { get; set; }
        public CourierPublicProfileDto Courier { get; set; }
    }
}

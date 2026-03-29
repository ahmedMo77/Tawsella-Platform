using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.CustomerDTOs;
using Tawsella.Domain.DTOs.OrderDTOs;

namespace Tawsella.Application.Contracts.Services
{
    public interface IPricingService
    {
        double CalculateDistance(decimal lat1, decimal lng1, decimal lat2, decimal lng2);
        PriceEstimateDto CalculateOrderPrice(CalculatePriceDto dto);
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs.CustomerDTOs;
using Tawsella.Application.DTOs.OrderDTOs;

namespace Tawsella.Infrastructure.Services
{
    public class PricingService : IPricingService
    {
        public PriceEstimateDto CalculateOrderPrice(CalculatePriceDto dto)
        {
            if (dto == null)
                throw new ValidationException("Invalid price calculation data");

            var distance = CalculateDistance(dto.PickupLatitude, dto.PickupLongitude, dto.DropoffLatitude, dto.DropoffLongitude);

            decimal basePrice = 30.00m;
            decimal distanceFee = (decimal)distance * 3.00m;

            decimal sizeMultiplier = dto.PackageSize?.ToLower() switch
            {
                "small" => 1.0m,
                "medium" => 1.3m,
                "large" => 1.6m,
                _ => 1.0m
            };

            var hour = DateTime.UtcNow.Hour;
            decimal timeMultiplier = (hour >= 12 && hour <= 14) || (hour >= 18 && hour <= 21) ? 1.2m : 1.0m;

            decimal totalPrice = (basePrice + distanceFee) * sizeMultiplier * timeMultiplier;

            // Calculate total
            decimal courierEarnings = totalPrice * 0.85m; // 85% to courier
            decimal platformCommission = totalPrice * 0.15m; // 15% platform

            // Estimate delivery time (30 km/h average speed)
            var timeInMinutes = (int)Math.Ceiling((distance / 30.0) * 60);

            return new PriceEstimateDto
            {
                EstimatedPrice = Math.Round(totalPrice, 2),
                Distance = Math.Round(distance, 1),
                BasePrice = basePrice,
                DistanceFee = Math.Round(distanceFee, 2),
                SizeMultiplier = sizeMultiplier,
                TimeMultiplier = timeMultiplier,
                CourierEarnings = Math.Round(courierEarnings, 2),
                PlatformCommission = Math.Round(platformCommission, 2),
                EstimatedDeliveryTime = $"{timeInMinutes}-{timeInMinutes + 15} minutes"
            };
        }

        public double CalculateDistance(decimal lat1, decimal lng1, decimal lat2, decimal lng2)
        {
            const double R = 6371;
            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lng2 - lng1));
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private double ToRadians(double degrees) => degrees * Math.PI / 180;

    }
}

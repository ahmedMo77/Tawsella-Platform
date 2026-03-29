namespace Tawsella.Application.DTOs.CustomerDTOs
{
    public class PriceEstimateDto
    {
        public decimal EstimatedPrice { get; set; }
        public double Distance { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DistanceFee { get; set; }
        public decimal SizeMultiplier { get; set; }
        public decimal TimeMultiplier { get; set; }
        public decimal CourierEarnings { get; set; }
        public decimal PlatformCommission { get; set; }
        public string EstimatedDeliveryTime { get; set; }
    }
}

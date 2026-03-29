using Tawsella.Application.Enums;

namespace Tawsella.Application.DTOs.CustomerDTOs
{
    public class CustomerProfileDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int CompletedOrders { get; set; }
    }
}

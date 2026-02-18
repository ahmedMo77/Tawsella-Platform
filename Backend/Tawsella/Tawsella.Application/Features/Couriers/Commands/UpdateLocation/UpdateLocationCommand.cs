using MediatR;

namespace Tawsella.Application.Features.Couriers.Commands.UpdateLocation
{
    public class UpdateLocationCommand : IRequest
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}

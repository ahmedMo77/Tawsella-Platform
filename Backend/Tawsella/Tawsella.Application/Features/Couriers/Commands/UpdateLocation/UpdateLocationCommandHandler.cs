using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;

namespace Tawsella.Application.Features.Couriers.Commands.UpdateLocation
{
    public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateLocationCommandHandler(
            ICourierRepository courierRepository,
            ICurrentUserService currentUserService)
        {
            _courierRepository = courierRepository;
            _currentUserService = currentUserService;
        }

        public async Task Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var courier = await _courierRepository.GetByIdAsync(courierId, cancellationToken);

            if (courier != null)
            {
                courier.CurrentLatitude = request.Latitude;
                courier.CurrentLongitude = request.Longitude;
                courier.LastLocationUpdate = DateTime.UtcNow;
                await _courierRepository.UpdateAsync(courier, cancellationToken);
            }
        }
    }
}

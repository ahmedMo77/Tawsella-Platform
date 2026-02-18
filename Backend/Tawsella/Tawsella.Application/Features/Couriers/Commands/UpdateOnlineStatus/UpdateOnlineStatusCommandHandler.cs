using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;

namespace Tawsella.Application.Features.Couriers.Commands.UpdateOnlineStatus
{
    public class UpdateOnlineStatusCommandHandler 
        : IRequestHandler<UpdateOnlineStatusCommand, UpdateOnlineStatusCommandResponse>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateOnlineStatusCommandHandler(
            ICourierRepository courierRepository,
            ICurrentUserService currentUserService)
        {
            _courierRepository = courierRepository;
            _currentUserService = currentUserService;
        }

        public async Task<UpdateOnlineStatusCommandResponse> Handle(
            UpdateOnlineStatusCommand request, 
            CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var courier = await _courierRepository.GetByIdAsync(courierId, cancellationToken);

            if (courier == null)
            {
                return new UpdateOnlineStatusCommandResponse
                {
                    Success = false
                };
            }

            courier.IsOnline = request.IsOnline;
            if (!request.IsOnline)
                courier.IsAvailable = false;

            await _courierRepository.UpdateAsync(courier, cancellationToken);

            return new UpdateOnlineStatusCommandResponse
            {
                Success = true
            };
        }
    }
}

using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;

namespace Tawsella.Application.Features.Couriers.Commands.UpdateCourierProfile
{
    public class UpdateCourierProfileCommandHandler 
        : IRequestHandler<UpdateCourierProfileCommand, UpdateCourierProfileCommandResponse>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateCourierProfileCommandHandler(
            ICourierRepository courierRepository,
            ICurrentUserService currentUserService)
        {
            _courierRepository = courierRepository;
            _currentUserService = currentUserService;
        }

        public async Task<UpdateCourierProfileCommandResponse> Handle( UpdateCourierProfileCommand request, CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var courier = await _courierRepository.GetCourierWithProfileAsync(courierId, cancellationToken);

            if (courier == null)
            {
                return new UpdateCourierProfileCommandResponse
                {
                    Success = false,
                    Message = "Courier not found"
                };
            }

            courier.User.FullName = request.FullName ?? courier.User.FullName;
            courier.User.PhoneNumber = request.PhoneNumber ?? courier.User.PhoneNumber;

            if (!string.IsNullOrEmpty(request.VehicleType))
                courier.VehicleType = request.VehicleType;

            courier.VehiclePlateNumber = request.VehiclePlateNumber ?? courier.VehiclePlateNumber;
            courier.MarkUpdated();

            await _courierRepository.UpdateAsync(courier, cancellationToken);

            return new UpdateCourierProfileCommandResponse
            {
                Success = true,
                Message = "Profile updated successfully"
            };
        }
    }
}

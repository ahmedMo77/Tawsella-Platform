using MediatR;

namespace Tawsella.Application.Features.Couriers.Commands.UpdateOnlineStatus
{
    public class UpdateOnlineStatusCommand : IRequest<UpdateOnlineStatusCommandResponse>
    {
        public bool IsOnline { get; set; }
    }
}

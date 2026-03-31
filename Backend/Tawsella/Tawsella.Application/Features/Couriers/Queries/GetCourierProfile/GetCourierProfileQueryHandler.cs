using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.DTOs.CourierDTOs;

namespace Tawsella.Application.Features.Couriers.Queries.GetCourierProfile
{
    public class GetCourierProfileQueryHandler 
        : IRequestHandler<GetCourierProfileQuery, GetCourierProfileQueryResponse>
    {
        private readonly ICourierRepository _courierRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetCourierProfileQueryHandler(
            ICourierRepository courierRepository, 
            IMapper _mapper,
            ICurrentUserService currentUserService)
        {
            _courierRepository = courierRepository;
            this._mapper = _mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GetCourierProfileQueryResponse> Handle(
            GetCourierProfileQuery request, 
            CancellationToken cancellationToken)
        {
            var courierId = _currentUserService.GetUserId();
            
            var courier = await _courierRepository.GetCourierWithProfileAsync(courierId, cancellationToken);

            var profile = _mapper.Map<GetCourierProfileQueryResponse>(courier);

            return profile;
        }
    }
}

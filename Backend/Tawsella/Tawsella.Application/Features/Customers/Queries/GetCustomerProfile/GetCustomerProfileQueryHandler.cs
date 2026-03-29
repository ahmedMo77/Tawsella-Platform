using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;

namespace Tawsella.Application.Features.Customers.Queries.GetCustomerProfile
{
    public class GetCustomerProfileQueryHandler 
        : IRequestHandler<GetCustomerProfileQuery, GetCustomerProfileQueryResponse>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetCustomerProfileQueryHandler(
            ICustomerRepository customerRepository, 
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<GetCustomerProfileQueryResponse> Handle(
            GetCustomerProfileQuery request, 
            CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();

            var customer = await _customerRepository.GetCustomerProfileAsync(customerId, cancellationToken);

            if (customer == null)
                throw new KeyNotFoundException($"Customer with ID {customerId} not found");

            var profile = _mapper.Map<Tawsella.Application.DTOs.CustomerDTOs.CustomerProfileDto>(customer);

            return new GetCustomerProfileQueryResponse
            {
                Profile = profile
            };
        }
    }
}

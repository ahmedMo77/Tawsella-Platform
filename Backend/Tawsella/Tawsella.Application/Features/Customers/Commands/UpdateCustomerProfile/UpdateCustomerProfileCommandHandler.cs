using AutoMapper;
using MediatR;
using Tawsella.Application.Contracts;
using Tawsella.Application.Interfaces;

namespace Tawsella.Application.Features.Customers.Commands.UpdateCustomerProfile
{
    public class UpdateCustomerProfileCommandHandler 
        : IRequestHandler<UpdateCustomerProfileCommand, UpdateCustomerProfileCommandResponse>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public UpdateCustomerProfileCommandHandler(
            ICustomerRepository customerRepository, 
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<UpdateCustomerProfileCommandResponse> Handle(
            UpdateCustomerProfileCommand request, 
            CancellationToken cancellationToken)
        {
            var customerId = _currentUserService.GetUserId();

            var customer = await _customerRepository.GetByIdAsync(customerId, cancellationToken);

            if (customer == null)
            {
                return new UpdateCustomerProfileCommandResponse
                {
                    Message = "Customer not found",
                    Success = false
                };
            }

            _mapper.Map(request, customer);
            customer.UpdatedAt = DateTime.UtcNow;
            await _customerRepository.UpdateAsync(customer, cancellationToken);

            return new UpdateCustomerProfileCommandResponse
            {
                Message = "Profile updated successfully",
                Success = true
            };
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Auth.Register.RegisterCourier
{
    public class RegisterCourierCommandHandler : IRequestHandler<RegisterCourierCommand, BaseToReturnDto>
    {
        private readonly ICourierRepository _courierRepo;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public RegisterCourierCommandHandler(ICourierRepository repo, IAuthService authServcie, IMapper mapper)
        {
            _mapper = mapper;
            _authService = authServcie;
            _courierRepo = repo;
        }
        public async Task<BaseToReturnDto> Handle(RegisterCourierCommand request, CancellationToken ct)
        {
            var nationalIdExists = await _courierRepo.GetCourierByNationalIdAsync(request.NationalId, ct);
            if (nationalIdExists != null)
                return new BaseToReturnDto { Message = "National ID already in use" };

            var registerDto = _mapper.Map<RegisterCourierDto>(request);

            var result = await _authService.RegisterCourierAsync(registerDto, ct);

            if (!result.Success)
                return new BaseToReturnDto { Message = result.Message };

            var courier = _mapper.Map<Courier>(request);
            courier.Id = result.Id;

            await _courierRepo.AddAsync(courier, ct);

            return new BaseToReturnDto { Success = true, Message = "Your request is under review" };
        }
    }
}
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Domain.Entities;
using Tawsella.Domain.Enums;

namespace Tawsella.Application.Features.Auth.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, BaseToReturnDto>
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public ConfirmEmailCommandHandler(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<BaseToReturnDto> Handle(ConfirmEmailCommand request, CancellationToken ct)
        {   
            var confirmEmailDto = _mapper.Map<ConfirmEmailDto>(request);
            var result = await _authService.ConfirmEmailAsync(confirmEmailDto, ct);

            return result;
        }
    }
}

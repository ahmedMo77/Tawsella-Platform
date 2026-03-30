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

namespace Tawsella.Application.Features.Auth.Password.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, BaseToReturnDto>
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        public ResetPasswordCommandHandler(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }
        public async Task<BaseToReturnDto> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            var resetPasswordDto = _mapper.Map<ResetPasswordDto>(request);
            var result = await _authService.ResetPasswordAsync(resetPasswordDto, ct);

            return result;
        }
    }
}

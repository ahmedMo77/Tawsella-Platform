using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
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

namespace Tawsella.Application.Features.Auth.Password.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, BaseToReturnDto>
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;

        public ChangePasswordCommandHandler(IAuthService authService, IMapper mapper, ICurrentUserService currentUser   )
        {
            _authService = authService;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<BaseToReturnDto> Handle(ChangePasswordCommand request, CancellationToken ct)
        {
            var changePasswordDto = _mapper.Map<ChangePasswordDto>(request);

            var userId = _currentUser.GetUserId();
            changePasswordDto.UserId = userId;

            var result = await _authService.ChangePasswordAsync(changePasswordDto, ct);

            return result;
        }
    }
}
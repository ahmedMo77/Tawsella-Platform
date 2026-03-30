using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs;
using Tawsella.Domain.Entities;
using Tawsella.Application.Contracts.Persistence;
using Tawsella.Application.Contracts.Services;

namespace Tawsella.Application.Features.Couriers.Commands.ApproveCourier
{
    public class ApproveCourierCommandHandler : IRequestHandler<ApproveCourierCommand, BaseToReturnDto>
    {
        private readonly ICourierRepository _courierRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly IEmailService _emailService;
        private readonly ICurrentUserService _currentUser;

        public ApproveCourierCommandHandler(
            ICourierRepository CourierRepo,
            IWalletRepository walletRepo,
            IEmailService emailService,
            ICurrentUserService currentUser)
        {
            _courierRepo = CourierRepo;
            _walletRepo = walletRepo;
            _emailService = emailService;
            _currentUser = currentUser;
        }

        public async Task<BaseToReturnDto> Handle(ApproveCourierCommand request, CancellationToken ct)
        {
            var courier = await _courierRepo.GetCourierWithUserAsync(request.CourierId, ct);

            if (courier == null || courier.IsApproved)
                return new BaseToReturnDto { Message = "This Courier is already approved or not found." };

            courier.IsApproved = true;
            courier.ApprovedAt = DateTime.UtcNow;
            courier.ApprovedBy = _currentUser.GetUserIdOrNull();
            courier.User.EmailConfirmed = true;

            await _courierRepo.UpdateAsync(courier, ct);

            // لازم اللوجيك دا يتنقل في الريبو بتاع المحفظة ونعمل كول من هنا بس
            var walletExists = await _walletRepo.GetByCourierIdAsync(request.CourierId, ct);

            if (walletExists == null)
            {
                var wallet = new Wallet
                {
                    Id = Guid.NewGuid().ToString(),
                    CourierId = request.CourierId,
                    Balance = 0,
                    PendingBalance = 0,
                    TotalEarnings = 0,
                    CreatedAt = DateTime.UtcNow
                };
                await _walletRepo.AddAsync(wallet, ct);
            }

            await _emailService.SendApprovedEmailAsync(courier.User.Email, ct);

            return new BaseToReturnDto { Success = true, Message = "Courier approved and wallet created successfully." };
        }
    }
}
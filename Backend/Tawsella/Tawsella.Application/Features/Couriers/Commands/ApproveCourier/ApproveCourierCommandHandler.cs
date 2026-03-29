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
        private readonly IEmailSender _emailService;

        public ApproveCourierCommandHandler(
            ICourierRepository CourierRepo,
            IWalletRepository walletRepo,
            IEmailSender emailService)
        {
            _courierRepo = CourierRepo;
            _walletRepo = walletRepo;
            _emailService = emailService;
        }

        public async Task<BaseToReturnDto> Handle(ApproveCourierCommand request, CancellationToken ct)
        {
            var courier = await _courierRepo.GetCourierWithUserAsync(request.CourierId, ct);


            if (courier == null || courier.IsApproved)
                return new BaseToReturnDto { Message = "This Courier is already approved or not found." };

            courier.IsApproved = true;
            courier.User.EmailConfirmed = true;

            await _courierRepo.UpdateAsync(courier, ct);

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

            await _emailService.SendEmailAsync(
                courier.User.Email,
                "Account Approved",
                "Congratulations! Your account is now active. You can start receiving orders now.");

            return new BaseToReturnDto { Success = true, Message = "Courier approved and wallet created successfully." };

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.DTOs.AuthDTOS;

namespace Tawsella.Application.Contracts.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string body);
        Task SendAdminInvitationEmail(CreateAdminEmailDto dto, CancellationToken ct);
    }
}

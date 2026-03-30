using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tawsella.Application.Contracts.Services;
using Tawsella.Application.DTOs.AuthDTOS;
using Tawsella.Application.Settings;
using Tawsella.Domain.Entities;

namespace Tawsella.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlContent)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                <div style='font-family: sans-serif; max-width: 600px; margin: 20px auto; border: 1px solid #eee; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
                    <div style='background: #2c3e50; color: #ffffff; padding: 20px; text-align: center;'>
                        <h1 style='margin: 0; font-size: 24px;'>Tawsella</h1>
                    </div>
                    <div style='padding: 30px; color: #333; line-height: 1.6;'>
                        {htmlContent}
                    </div>
                    <div style='background: #f8f9fa; padding: 15px; text-align: center; font-size: 12px; color: #999;'>
                        This is an automated message from Tawsella System.
                    </div>
                </div>"
                };

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}, Subject: {Subject}", email, subject);
                throw;
            }
        }

        public async Task SendVerificationCodeAsync(string email, string code, CancellationToken ct)
        {
            string body = $@"
                <h2 style='color: #2c3e50;'>Verify Your Email</h2>
                <p>Thank you for signing up! Please use the following code to complete your registration:</p>
                <div style='background: #f4f4f4; padding: 20px; font-size: 32px; font-weight: bold; text-align: center; letter-spacing: 10px; color: #2c3e50; border: 1px dashed #2c3e50;'>
                    {code}
                </div>
                <p style='color: #7f8c8d; font-size: 14px; margin-top: 20px;'>This code will expire in 15 minutes.</p>";


            await SendEmailAsync(email, "Confirm Your Email", body);
        }

        public async Task SendAdminInvitationEmail(CreateAdminEmailDto createAdminEmail, CancellationToken ct)
        {
            var subject = "Welcome to Tawsella Team!";
            var body = $@"
        <h2 style='color: #2c3e50;'>Welcome to Tawsella Team!</h2>
        <p>Hello <strong>{createAdminEmail.FullName}</strong>,</p>
        <p>A new administrator account has been created for you.</p>
        <div style='background: #f4f7f6; padding: 15px; border-radius: 5px; margin: 20px 0;'>
            <p><strong>Email:</strong> {createAdminEmail.Email}</p>
            <p><strong>Temp Password:</strong> <span style='color: #e74c3c; font-weight: bold;'>{createAdminEmail.Password}</span></p>
        </div>
        <p>Please change your password after your first login.</p>";

            await SendEmailAsync(createAdminEmail.Email, subject, body);
        }

        public async Task SendApprovedEmailAsync(string email, CancellationToken ct)
        {
            var body = $@"
                <p>Congratulations! Your account has been approved and is now active.</p>
                <p>You can now log in and start receiving orders.</p>";
            await SendEmailAsync(email, "Account Approved", body);
        }
    }
}

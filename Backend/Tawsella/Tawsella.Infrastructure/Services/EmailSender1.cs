using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tawsella.Application.Settings;
using Tawsella.Application.Contracts.Services;

namespace Tawsella.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger)
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
    }
}

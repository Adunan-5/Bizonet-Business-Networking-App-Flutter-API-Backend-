using Bizonet.Api.Configurations;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Bizonet.Api.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string email, string otpCode, string purpose);
    }

    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtp;

        public EmailService(IOptions<SmtpSettings> smtpOptions)
        {
            _smtp = smtpOptions.Value;
        }

        public async Task SendOtpEmailAsync(string email, string otpCode, string purpose)
        {
            string subject;
            string html;

            if (purpose == "RegisterEmailOtp")
            {
                subject = "OTP for Registration";
                html = $@"
                    <div style='font-family:Arial'>
                        <h2>Bizonet OTP Verification</h2>
                        <p>Your OTP for account verification is:</p>
                        <h1 style='letter-spacing:5px'>{otpCode}</h1>
                        <p>This OTP will expire in 5 minutes.</p>
                    </div>";
            }
            else if (purpose == "LoginEmailOtp")
            {
                subject = "OTP for Login";
                html = $@"
                    <div style='font-family:Arial'>
                        <h2>Bizonet Login OTP</h2>
                        <p>Your OTP for login is:</p>
                        <h1 style='letter-spacing:5px'>{otpCode}</h1>
                        <p>This OTP will expire in 5 minutes.</p>
                        <p>If you didn’t request this, please ignore.</p>
                    </div>";
            }
            else
            {
                subject = "Your Bizonet OTP";
                html = $@"
                    <div style='font-family:Arial'>
                        <h2>Bizonet OTP</h2>
                        <p>Your OTP is:</p>
                        <h1 style='letter-spacing:5px'>{otpCode}</h1>
                        <p>This OTP will expire in 5 minutes.</p>
                    </div>";
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtp.FromName, _smtp.FromEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            message.Body = new BodyBuilder
            {
                HtmlBody = html
            }.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync(_smtp.Host, _smtp.Port, true);
            await client.AuthenticateAsync(_smtp.Username, _smtp.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

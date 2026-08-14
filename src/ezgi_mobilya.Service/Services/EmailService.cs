using ezgi_mobilya.Core.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace ezgi_mobilya.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public bool IsConfigured => !string.IsNullOrWhiteSpace(GetHost());

        public async Task SendContactNotificationAsync(ContactMessage message, CancellationToken cancellationToken = default)
        {
            var host = GetHost();
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new InvalidOperationException("SMTP host ayarı eksik. İletişim e-postası gönderilemedi.");
            }

            var toAddress = SanitizeHeader(_configuration["SiteContact:Email"]);
            if (string.IsNullOrWhiteSpace(toAddress) || !MailboxAddress.TryParse(toAddress, out _))
            {
                throw new InvalidOperationException("SiteContact:Email geçerli bir alıcı adresi değil.");
            }

            var fromAddress = SanitizeHeader(_configuration["Smtp:From"]);
            if (string.IsNullOrWhiteSpace(fromAddress))
            {
                fromAddress = SanitizeHeader(_configuration["Smtp:User"]);
            }

            if (string.IsNullOrWhiteSpace(fromAddress) || !MailboxAddress.TryParse(fromAddress, out _))
            {
                throw new InvalidOperationException("SMTP gönderen adresi (Smtp:From veya Smtp:User) geçersiz.");
            }

            var fromName = SanitizeHeader(_configuration["Smtp:FromName"]);
            if (string.IsNullOrWhiteSpace(fromName))
            {
                fromName = "Ezgi Craft";
            }

            var visitorName = SanitizeHeader(message.FullName);
            var visitorEmail = SanitizeHeader(message.Email);
            var subject = SanitizeHeader(message.Subject);
            if (string.IsNullOrWhiteSpace(subject))
            {
                subject = "İletişim formu";
            }

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(fromName, fromAddress));
            mimeMessage.To.Add(MailboxAddress.Parse(toAddress));
            mimeMessage.Subject = $"[Ezgi Craft] {subject}";

            if (MailboxAddress.TryParse(visitorEmail, out var replyTo))
            {
                mimeMessage.ReplyTo.Add(new MailboxAddress(visitorName, replyTo.Address));
            }

            mimeMessage.Body = new TextPart("plain")
            {
                Text =
                    $"""
                    Ezgi Craft sitesinden yeni bir iletişim formu mesajı alındı.

                    Ad Soyad: {visitorName}
                    E-posta: {visitorEmail}
                    Konu: {subject}

                    Mesaj:
                    {message.Message}

                    ---
                    Bu e-posta otomatik olarak gönderilmiştir.
                    """
            };

            var port = GetPort();
            var enableSsl = GetEnableSsl();
            var socketOptions = port == 465
                ? SecureSocketOptions.SslOnConnect
                : enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

            using var client = new SmtpClient();
            client.Timeout = 20000;

            await client.ConnectAsync(host, port, socketOptions, cancellationToken);

            var user = _configuration["Smtp:User"];
            var password = _configuration["Smtp:Password"];
            if (!string.IsNullOrWhiteSpace(user))
            {
                await client.AuthenticateAsync(user, password ?? string.Empty, cancellationToken);
            }

            await client.SendAsync(mimeMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("İletişim formu e-postası gönderildi. Alıcı: {To}", toAddress);
        }

        private string? GetHost() => _configuration["Smtp:Host"];

        private int GetPort()
        {
            if (int.TryParse(_configuration["Smtp:Port"], out var port) && port > 0)
            {
                return port;
            }

            return 587;
        }

        private bool GetEnableSsl()
        {
            if (bool.TryParse(_configuration["Smtp:EnableSsl"], out var enabled))
            {
                return enabled;
            }

            return true;
        }

        private static string SanitizeHeader(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.Replace("\r", " ", StringComparison.Ordinal)
                .Replace("\n", " ", StringComparison.Ordinal)
                .Trim();
        }
    }
}

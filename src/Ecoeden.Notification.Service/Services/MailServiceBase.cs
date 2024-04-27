using Ecoeden.Notification.Service.Configurations;
using Ecoeden.Notification.Service.Entities;
using Ecoeden.Notification.Service.Extensions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;

namespace Ecoeden.Notification.Service.Services
{
    public abstract class MailServiceBase
    {
        protected readonly EmailSettingsOption _settings;
        protected readonly ILogger _logger;

        public MailServiceBase(IOptions<EmailSettingsOption> settings, ILogger logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        protected async Task<SmtpClient> CreateMailClient()
        {
            var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_settings.Server, _settings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
            }
            catch (Exception ex)
            {
                _logger.Here().Information("Failed to establish connection to SMTP server. {@stackTrace}", ex);
            }

            _logger.Here().Information("Mail client established {@clientDetails}", _settings);
            return client;
        }

        protected abstract MimeMessage ProcessMessage(NotificationHistory notification);

        protected abstract string ReadEmailTemplateText(string templateName);
    }
}

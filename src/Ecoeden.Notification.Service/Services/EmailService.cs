using Ecoeden.Notification.Service.Configurations;
using Ecoeden.Notification.Service.Data;
using Ecoeden.Notification.Service.Entities;
using Ecoeden.Notification.Service.Extensions;
using Ecoeden.Notification.Service.Models.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace Ecoeden.Notification.Service.Services
{
    public sealed class EmailService : MailServiceBase, IEmailService
    {
        private readonly NotificationDbContext _dbContext;

        public EmailService(IOptions<EmailSettingsOption> settings, 
            ILogger logger, 
            NotificationDbContext dbContext)
            : base(settings, logger)
        {
            _dbContext = dbContext;
        }

        public async Task SendEmailAsync()
        {
            var notificationsToProcess = await _dbContext.NotificationHistories
                .Where(x => !x.IsPublished)
                .ToListAsync();

            if (notificationsToProcess == null || notificationsToProcess.Count  == 0)
            {
                return;
            }

            var mailClient = await CreateMailClient();

            foreach (var notification in notificationsToProcess)
            {
                _logger.Here().Information("Message processing {@subject}", notification.Subject);
                var mail = ProcessMessage(notification);
                try
                {
                    await mailClient.SendAsync(mail);
                    notification.IsPublished = true;
                    notification.PublishTime = DateTime.UtcNow;
                    _dbContext.NotificationHistories.Update(notification);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.Here().Error("{@message} - {@trace}", e.Message, e.StackTrace);
                }
            }
        }

        protected override MimeMessage ProcessMessage(NotificationHistory notification)
        {
            var emailTemplateText = ReadEmailTemplateText($"{notification.TemplateName}.html");
            var emailFields = JsonConvert.DeserializeObject<List<TemplateFields>>(notification.Data);
            var builder = new BodyBuilder();

            var emailBuilder = new StringBuilder(emailTemplateText);

            foreach (var field in emailFields)
            {
                _logger.Here().Information($"field - {field.Key}, value - {field.Value}");
                emailBuilder.Replace(field.Key, field.Value);
            }

            var email = new MimeMessage();
            email.To.Add(MailboxAddress.Parse(notification.RecipientEmail));
            email.Subject = notification.Subject;
            email.Sender = MailboxAddress.Parse(_settings.CompanyAddress);
            builder.HtmlBody = emailBuilder.ToString();
            email.Body = builder.ToMessageBody();

            return email;
        }

        protected override string ReadEmailTemplateText(string templateName)
        {
            return File.ReadAllText($"./Templates/{templateName}");
        }
    }
}

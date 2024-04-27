using Contracts.Events;
using Ecoeden.Notification.Service.Configurations;
using Ecoeden.Notification.Service.Data;
using Ecoeden.Notification.Service.Entities;
using Ecoeden.Notification.Service.Extensions;
using Ecoeden.Notification.Service.Models.Constants;
using Ecoeden.Notification.Service.Models.Notifications;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;


namespace Ecoeden.Notification.Service.EventBus.Consumers
{
    public sealed class UserInvitationSentConsumer : IConsumer<UserInvitationSent>
    {
        private readonly ILogger _logger;
        private readonly EmailTemplates _emailTemplates;
        private readonly NotificationDbContext _dbContext;

        public UserInvitationSentConsumer(ILogger logger, 
            NotificationDbContext dbContext,
            IOptions<EmailTemplates> emailTemplates)
        {
            _logger = logger;
            _emailTemplates = emailTemplates.Value;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<UserInvitationSent> context)
        {
            _logger.Here().MethodEnterd();
            _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing started for event {Event}", typeof(UserInvitationSent).Name);

            try
            {
                NotificationHistory notificationHistory = new()
                {
                    Subject = EmailSubjects.UserInvitation,
                    Data = GetEmailData(context.Message),
                    CorrelationId = context.Message.CorrelationId,
                    IsPublished = false,
                    TemplateName = _emailTemplates.UserInivite,
                    RecipientEmail = context.Message.Email
                };

                _dbContext.Add(notificationHistory);
                await _dbContext.SaveChangesAsync();

                _logger.Here()
                    .WithCorrelationId(context.Message.CorrelationId)
                    .Information("Notification history updated");
            }
            catch(Exception ex)
            {
                _logger.Here()
                    .WithCorrelationId(context.Message.CorrelationId)
                    .Error(ex.Message);
            }
        }

        private static string GetEmailData(UserInvitationSent message)
        {
            List<TemplateFields> templateFields = new()
            {
                new("[firstname]", message.FirstName),
                new("[lastname]", message.LastName),
                new("[acceptanceLink]", "http://ecoeden.com/")
            };
            return JsonConvert.SerializeObject(templateFields);
        }
    }
}

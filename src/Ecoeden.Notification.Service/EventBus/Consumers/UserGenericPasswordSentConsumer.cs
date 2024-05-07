using Contracts.Events;
using Ecoeden.Notification.Service.Configurations;
using Ecoeden.Notification.Service.Data;
using Ecoeden.Notification.Service.Entities;
using Ecoeden.Notification.Service.Extensions;
using Ecoeden.Notification.Service.Models.Constants; 
using Ecoeden.Notification.Service.Models.Notifications;
using Newtonsoft.Json;
using MassTransit;
using Serilog;
using Microsoft.Extensions.Options;

namespace Ecoeden.Notification.Service.EventBus.Consumers;
public sealed class UserGenericPasswordSentConsumer : IConsumer<UserGenericPasswordSent>
{
    private readonly ILogger _logger;
    private readonly EmailTemplates _emailTemplates;
    private readonly NotificationDbContext _dbContext;

    public UserGenericPasswordSentConsumer(ILogger logger, 
        IOptions<EmailTemplates> emailTemplates, 
        NotificationDbContext dbContext)
    {
        _logger = logger;
        _emailTemplates = emailTemplates.Value;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<UserGenericPasswordSent> context)
    {
        _logger.Here().MethodEnterd();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started for event {Event}", typeof(UserGenericPasswordSent).Name);

        try
        {
            NotificationHistory notificationHistory = new()
            {
                Subject = EmailSubjects.UserPasswordSent,
                Data = GetEmailData(context.Message),
                CorrelationId = context.Message.CorrelationId,
                IsPublished = false,
                TemplateName = _emailTemplates.UserPassword,
                RecipientEmail = context.Message.Email
            };

            _dbContext.Add(notificationHistory);
            await _dbContext.SaveChangesAsync();

            _logger.Here()
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Notification history updated");
        }
        catch (Exception ex)
        {
            _logger.Here()
                .WithCorrelationId(context.Message.CorrelationId)
                .Error(ex.Message);
        }
    }

    private static string GetEmailData(UserGenericPasswordSent message)
    {
        List<TemplateFields> templateFields = new()
        {
            new("[name]", message.Name),
            new("[defaultPassword]", message.DefaultPassword)
        };
        return JsonConvert.SerializeObject(templateFields);
    }
}

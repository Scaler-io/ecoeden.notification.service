using Contracts.Events;
using Ecoeden.Notification.Service.Configurations;
using Ecoeden.Notification.Service.Data;
using Ecoeden.Notification.Service.Entities;
using Ecoeden.Notification.Service.Extensions;
using Ecoeden.Notification.Service.Models.Constants;
using Ecoeden.Notification.Service.Models.Notifications;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Ecoeden.Notification.Service.EventBus.Consumers;

public sealed class UserInvitationSentConsumer : IConsumer<UserInvitationSent>
{
    private readonly ILogger _logger;
    private readonly EmailTemplates _emailTemplates;
    private readonly NotificationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public UserInvitationSentConsumer(ILogger logger,
        NotificationDbContext dbContext,
        IOptions<EmailTemplates> emailTemplates,
        IConfiguration configuration)
    {
        _logger = logger;
        _emailTemplates = emailTemplates.Value;
        _dbContext = dbContext;
        _configuration = configuration;
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
            var identityBaseUrl = _configuration["InfrastructureSettings:identityBaseUrl"];
            NotificationHistory notificationHistory = new()
            {
                Subject = EmailSubjects.UserInvitation,
                Data = GetEmailData(context.Message, identityBaseUrl),
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

    private static string GetEmailData(UserInvitationSent message, string identityUrl)
    {
        JObject obj = JObject.Parse(message.AdditionalProperties.ToString());
        string token = obj["token"].Value<string>();

        Uri url = new Uri($"{identityUrl}/account/emailverification?userId={message.UserId}&token={token}");
        List<TemplateFields> templateFields = new()
        {
            new("[firstname]", message.FirstName),
            new("[lastname]", message.LastName),
            new("[acceptanceLink]", url.ToString())
        };
        return JsonConvert.SerializeObject(templateFields);
    }
}

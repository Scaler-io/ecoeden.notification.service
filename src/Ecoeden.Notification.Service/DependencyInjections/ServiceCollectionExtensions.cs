using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Ecoeden.Notification.Service.Configurations;
using Ecoeden.Notification.Service.EventBus.Consumers;
using MassTransit.Definition;
using Ecoeden.Notification.Service.Services;

namespace Ecoeden.Notification.Service.DependencyInjections
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            var logger = Logging.GetLogger(configuration);
            services.AddSingleton(logger);


            services.Configure<AppOptions>(configuration.GetSection(AppOptions.OptionName));
            services.Configure<EmailSettingsOption>(configuration.GetSection(EmailSettingsOption.OptionName));
            services.Configure<EmailTemplates>(configuration.GetSection(EmailTemplates.OptionName));

            services.AddMassTransit(config =>
            {
                config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter());
                config.AddConsumersFromNamespaceContaining<UserInvitationSentConsumer>();
                config.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitmq = configuration.GetSection("EventBus").Get<RabbitMqOptions>();
                    cfg.Host(rabbitmq.Host, "/", host =>
                    {
                        host.Username(rabbitmq.Username);
                        host.Password(rabbitmq.Password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}

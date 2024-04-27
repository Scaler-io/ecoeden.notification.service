// See https://aka.ms/new-console-template for more information
using Ecoeden.Notification.Service;
using Ecoeden.Notification.Service.BackgroundJobs;
using Ecoeden.Notification.Service.Data;
using Ecoeden.Notification.Service.DependencyInjections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

ILogger logger = null;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        logger = Logging.GetLogger(configuration);

        services.ConfigurationSettings();
        services.AddApplicationServices(configuration)
                .AddDataServices(configuration);

        services.AddHostedService<EventBusStarterJob>();
        services.AddHostedService<EmailProcessingJob>();
        

    }).UseSerilog(logger).Build();

var dbContext = host.Services.GetRequiredService<NotificationDbContext>();

try
{
    await dbContext.Database.MigrateAsync();
    await host.RunAsync();
}
finally
{
    Log.CloseAndFlush();
}
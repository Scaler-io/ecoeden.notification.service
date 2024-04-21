// See https://aka.ms/new-console-template for more information
using Ecoeden.Notification.Service;
using Ecoeden.Notification.Service.DependencyInjections;
using Ecoeden.Notification.Service.Extensions;
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
        services.AddApplicationServices(configuration);
    }).UseSerilog(logger).Build();


try
{
    logger.Here().Information("App started");
    await host.RunAsync();
}
finally
{
    Log.CloseAndFlush();
}
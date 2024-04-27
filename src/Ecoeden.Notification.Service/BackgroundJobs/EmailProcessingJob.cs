using Ecoeden.Notification.Service.Configurations;
using Ecoeden.Notification.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Ecoeden.Notification.Service.BackgroundJobs
{
    public class EmailProcessingJob : BackgroundService
    {
        private readonly AppOptions _settings;
        private readonly IServiceProvider _serviceProvider;

        public EmailProcessingJob(IOptions<AppOptions> appOption,
            IServiceProvider serviceProvider)
        {
            _settings = appOption.Value;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                System.Console.WriteLine($"Next jon running {DateTime.Now.AddSeconds(10)}");
                using var scope = _serviceProvider.CreateScope();

                IEmailService emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                await emailService.SendEmailAsync();

                await Task.Delay(
                    _settings.IntervalUnit == "ss" 
                    ? TimeSpan.FromSeconds(Convert.ToInt32(_settings.NotificationProcessInterval))
                    : TimeSpan.FromMinutes(Convert.ToInt32(_settings.NotificationProcessInterval)), stoppingToken);
            }
        }
    }
}

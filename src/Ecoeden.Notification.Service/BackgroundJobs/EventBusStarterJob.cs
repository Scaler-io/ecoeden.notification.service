using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Ecoeden.Notification.Service.BackgroundJobs
{
    public sealed class EventBusStarterJob : BackgroundService
    {
        private readonly IBusControl _busControl;

        public EventBusStarterJob(IBusControl busControl)
        {
            _busControl = busControl;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _busControl.StartAsync(stoppingToken);
            if (stoppingToken.IsCancellationRequested)
            {
                await _busControl.StopAsync(stoppingToken);
            }
            Console.WriteLine("Working");
        }
    }
}

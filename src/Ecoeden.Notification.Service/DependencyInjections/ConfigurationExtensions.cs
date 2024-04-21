using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ecoeden.Notification.Service.DependencyInjections
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection ConfigurationSettings(this IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .Build();

            services.AddSingleton<IConfiguration>(configuration);

            return services;
        }
    }
}

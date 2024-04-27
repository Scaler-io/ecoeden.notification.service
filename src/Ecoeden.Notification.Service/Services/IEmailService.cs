using Ecoeden.Notification.Service.Models.Enums;

namespace Ecoeden.Notification.Service.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync();
    }
}

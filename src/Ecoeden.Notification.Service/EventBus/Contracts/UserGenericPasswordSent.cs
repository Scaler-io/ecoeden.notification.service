using Ecoeden.Notification.Service.EventBus.Contracts;
using Ecoeden.Notification.Service.Models.Enums;

namespace Contracts.Events;
public sealed class UserGenericPasswordSent : NotificationEvent
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string DefaultPassword { get; set; }
    public override NotificationType NotificationType { get; set; } = NotificationType.UserGenericPasswordSent;
}

using Ecoeden.Notification.Service.Models.Enums;

namespace Ecoeden.Notification.Service.EventBus.Contracts
{
    public abstract class NotificationEvent
    {
        public DateTime CreatedAt { get; set; }
        public string CorrelationId { get; set; }
        public object AdditionalProperties { get; set; }
        public abstract NotificationType NotificationType { get; set; }
    }
}

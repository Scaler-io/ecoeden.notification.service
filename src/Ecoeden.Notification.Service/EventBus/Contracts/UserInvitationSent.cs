using Ecoeden.Notification.Service.EventBus.Contracts;
using Ecoeden.Notification.Service.Models.Enums;

namespace Contracts.Events;

public sealed class UserInvitationSent : NotificationEvent
{
    public string UserId { get; private set; }
    public string UserName { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; set; }
    public string Email { get; private set; }
    public override NotificationType NotificationType { get; set; } = NotificationType.UserInvitation;
}

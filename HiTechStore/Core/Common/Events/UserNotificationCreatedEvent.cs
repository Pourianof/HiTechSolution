namespace HiTechStore.Core.Common.Events;

public class UserNotificationCreatedEvent : IEvent
{
    public string EventName { get; set; } = "UserNotifiactionCreated";
    required public Guid NotificationId { get; set; }
}
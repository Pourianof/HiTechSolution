namespace HiTechStore.Core.Common.Events;

public class PermissionChangedEvent : IEvent
{
    public string EventName { get; set; } = "PermissionChanged";
    required public string TargetUserId { get; set; }
}
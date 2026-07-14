using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Core.Dto.UserNotification;

[MapTo<Models.UserNotification>]
public class CreateNotificationDto
{
    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;

    [MapToProperty(nameof(Models.UserNotification.OwnerId))]
    required public string ForUserId { get; set; }

    [MapToProperty(nameof(Models.UserNotification.Type))]
    required public string NotificationType { get; set; }
}
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Core.Dto.UserNotification;

[MapFrom<Models.UserNotification>]
public class UserNotificationDto
{
    public Guid Id { get; set; }

    public string? Type { get; set; }

    public string? Title { get; set; }

    public string? Body { get; set; }

    public string? OwnerId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; } = default;
}
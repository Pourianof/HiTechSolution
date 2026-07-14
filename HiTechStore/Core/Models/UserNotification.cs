
namespace HiTechStore.Core.Models;

public class UserNotification : IModel
{
    public Guid Id { get; set; }

    public string? Type { get; set; }

    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;

    public string? OwnerId { get; set; }
    virtual public User? Owner { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; } = default;
    public bool IsRead => ReadAt is not null;
}


namespace HiTechStore.Core.Common.Interfaces.Presentation;

public interface ICurrentUserProvider
{
    public string? UserId { get; init; }
    public bool IsAuthorized { get; }
}


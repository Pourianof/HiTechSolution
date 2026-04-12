namespace HiTechStore.Core.Auth;

public interface ICurrentUserProvider
{
    public string? UserId { get; init; }
    public bool IsAuthorized { get; }
}


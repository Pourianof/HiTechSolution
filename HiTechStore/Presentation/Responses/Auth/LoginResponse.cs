
using HiTechStore.Core.Dto.Auth;

namespace HiTechStore.Presentation.Responses;

public class LoginResponse
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserDto? User { get; set; }
}
namespace HiTechStore.Data.DTOs.Authorization;

public class LoginResponseDto
{
    public string? Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserDto? User { get; set; }
}
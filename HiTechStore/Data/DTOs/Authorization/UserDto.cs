namespace HiTechStore.Data.DTOs.Authorization;

public class UserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? AvatarUrl { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}

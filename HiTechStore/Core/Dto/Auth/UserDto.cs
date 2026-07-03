using HiTechStore.Core.Dto.Permission;
using HiTechStore.Core.Models;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Core.Dto.Auth;

[MapFrom<User>]
public class UserDto
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? AvatarUrl { get; set; }
    public IEnumerable<string>? Roles { get; set; }
    public IEnumerable<UserPermissionDto>? Permissions { get; set; }
}

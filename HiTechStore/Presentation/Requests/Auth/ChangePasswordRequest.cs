using System.ComponentModel.DataAnnotations;

using HiTechStore.Core.Dto.Auth;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Presentation.Requests.Auth;

[MapTo<ChangePasswordDto>]
public class ChangePaswordRequest
{
    [Required]
    public string? OldPassword { get; set; }
    [Required]
    public string? NewPassword { get; set; }
    [Required]
    public string? PasswordConfirmation { get; set; }
}
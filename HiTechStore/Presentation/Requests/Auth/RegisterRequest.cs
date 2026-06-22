namespace HiTechStore.Presentation.Auth;

using System.ComponentModel.DataAnnotations;

using HiTechStore.Core.Dto.Auth;
using HiTechStore.Core.Models;
using HiTechStore.Helpers.AutoMapper;

[MapTo<RegisterDto>]
public class RegisterRequest
{
    [MinLength(3)]
    public string? Username { get; set; }
    [EmailAddress]
    [Required]
    public string? Email { get; set; }
    [Required]
    [MinLength(6)]
    public string? Password { get; set; }
    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string? ConfirmPassword { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(20)]
    public string? FirstName { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(20)]
    public string? LastName { get; set; }
    [RegularExpression(@$"^(?i)({IdentityRoles.Admin}|{IdentityRoles.Manager}|{IdentityRoles.User})$", ErrorMessage = "Invalid role")]
    public string? Role { get; set; }
}

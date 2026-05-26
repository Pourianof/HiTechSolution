using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Presentation.Requests.User;

public class ResetPasswordRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Token { get; set; }

    [Required]
    [MinLength(6)]
    public string? NewPassword { get; set; }

    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    public string? PasswordConfirmation { get; set; }
}

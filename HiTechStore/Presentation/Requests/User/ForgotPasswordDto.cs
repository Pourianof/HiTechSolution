using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Presentation.Requests.User;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    [Url]
    public string? ReturnUrl { get; set; }
}

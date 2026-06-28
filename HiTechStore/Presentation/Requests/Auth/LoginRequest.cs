using System.ComponentModel.DataAnnotations;

using HiTechStore.Core.Dto.Auth;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Presentation.Auth
{
    [MapTo<LoginDto>]
    public class LoginRequest
    {
        [MinLength(3)]
        [MaxLength(20)]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Username can only contain letters, numbers(0-9), and underscores(_).")]
        public string? Username { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [MinLength(6)]
        public string? Password { get; set; }
    }
}
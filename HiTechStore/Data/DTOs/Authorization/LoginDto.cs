using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Data.DTOs.Authorization
{
    public class LoginDto
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
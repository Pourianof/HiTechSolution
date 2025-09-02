using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Data.DTOs.Authorization
{
    public class LoginDto
    {
        [Required]
        [MinLength(3)]
        public string? Username { get; set; }
        [Required]
        [MinLength(6)]
        public string? Password { get; set; }
    }
}
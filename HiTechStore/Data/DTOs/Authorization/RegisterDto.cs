using System.ComponentModel.DataAnnotations;

using HiTechStore.Models;

namespace HiTechStore.Data.DTOs.Authorization
{
    public class RegisterDto
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
}
using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Infrastructure.Data.DTOs.Discount;

public class ScriptCheckingDto
{
    [Required]
    [MinLength(5)]
    public string? Script { get; set; }
}
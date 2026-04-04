using System.ComponentModel.DataAnnotations;

namespace HiTechStore.Data.DTOs.Discount;

public class ScriptCheckingDto
{
    [Required]
    [MinLength(5)]
    public string? Script { get; set; }
}
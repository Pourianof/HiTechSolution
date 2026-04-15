using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiTechStore.ApiTokenHandler.Core.Models;

[Table("RefreshTokens")]
public class RefreshToken
{
    [Key]
    public string? Token { get; set; }
    public string? UserId { get; set; }
    public DateTime ExpirateAt { get; set; }
}

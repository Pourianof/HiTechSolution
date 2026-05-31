using System.ComponentModel.DataAnnotations;

using HiTechStore.Infrastructure.Data.DTOs.Validations;

namespace HiTechStore.Presentation.Requests.Comment;

public class ResponseCommentRequest
{
    [Required]
    [NonZeroPositiveNumber]
    public int ResponsedCommentId { get; set; }
    [Required]
    [MinLength(1)]
    public string? Text { get; set; }
}
using System.ComponentModel.DataAnnotations;

using HiTechStore.Core.Dto.ProductComment;
using HiTechStore.Infrastructure.Data.DTOs.Validations;
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Presentation.Requests.Comment;

[MapTo<ProductCommentCreationDto>]
public class CommentCreationRequest
{
    [Required]
    [MinLength(3)]
    public string? Text { get; set; }
    [Required]
    [NonZeroPositiveNumber]
    public int Rate { get; set; }
}
using HiTechStore.Helpers.AutoMapper;

namespace HiTechStore.Core.Dto.Comment;

[MapTo<Core.Models.Comment>]
public class CommentCreationDto
{
    public int? ProductId { get; set; }
    public int? ParentId { get; set; }
    public string? Text { get; set; }
    public int? RateId { get; set; }
}
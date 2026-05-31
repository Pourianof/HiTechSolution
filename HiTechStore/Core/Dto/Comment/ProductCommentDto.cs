using HiTechStore.Helpers.AutoMapper;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Dto.Comment;


[MapFrom<Core.Models.Comment>]
public class CommentDto
{
    public int CommentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public CommentUserDto? User { get; set; }
    public int? ProductId { get; set; }
    public IEnumerable<CommentDto>? Responses { get; set; }
    public int ResponsesCount { get; set; }
    public string? Text { get; set; }
    [MapFromProperty([nameof(Models.Comment.Rate), nameof(Models.Comment.Rate.Score)])]
    public int? Rate { get; set; }
}


[MapFrom<User>]
public class CommentUserDto
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? ProfileAvatar { get; set; }
}
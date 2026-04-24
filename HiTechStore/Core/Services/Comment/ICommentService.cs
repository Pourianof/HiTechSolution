using HiTechStore.Core.Dto.Comment;
using HiTechStore.Data.DTOs;

namespace HiTechStore.Core.Services.Comment;

public interface ICommentService
{
    Task<Models.Comment> CreateComment(CommentCreationDto commentDto);
    Task<CommentDto?> GetCommentById(int id);
    Task RemoveComment(int id);
}


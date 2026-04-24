using HiTechStore.Core.Dto.Comment;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface ICommentRepository : IRepository<Comment, CommentDto>
{
    Task<PagedResultDto<CommentDto>> GetCommentsOfProduct(int productId, BaseQuery? query, string? userId = default);
    Task<CommentDto?> GetCommentOfUserForProduct(int productId, string userId);
}
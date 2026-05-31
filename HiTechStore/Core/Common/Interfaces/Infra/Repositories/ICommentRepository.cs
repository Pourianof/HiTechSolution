using HiTechStore.Core.Dto.Comment;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface ICommentRepository : IRepository<Comment, CommentDto>
{
    Task<PagedResultDto<CommentDto>> GetCommentsOfProduct(int productId, BaseQuery? query, string? userId = default);
    Task<CommentDto?> GetCommentOfUserForProduct(int productId, string userId);
}
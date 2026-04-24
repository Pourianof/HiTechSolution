using HiTechStore.Core.Dto.Comment;
using HiTechStore.Core.Dto.ProductComment;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.Queries;

namespace HiTechStore.Core.Services.ProductComment;

public interface IProductCommentService
{
    Task<CommentDto> AddCommentForProduct(int productId, ProductCommentCreationDto commentCreationDto);
    Task RemoveCommentOfProduct(int productId, int CommentId);
    Task<CommentDto> UpdateCommentOfProduct(int productId, int CommentId);
    Task<PagedResultDto<CommentDto>> GetProductComments(int productId, BaseQuery query);
}
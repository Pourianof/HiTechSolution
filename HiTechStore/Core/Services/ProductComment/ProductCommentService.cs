using AutoMapper;

using HiTechStore.Core.Auth;
using HiTechStore.Core.Dto.Comment;
using HiTechStore.Core.Dto.ProductComment;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Core.Services.Comment;
using HiTechStore.Core.Services.Product;
using HiTechStore.Core.Services.ProductScore;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.DTOs.Product;
using HiTechStore.Data.Queries;

namespace HiTechStore.Core.Services.ProductComment;

public class ProductCommentService(
    ICommentService commentService,
    IProductService productService,
    IAuthorizationService authorizationService,
    ICurrentUserProvider userProvider,
    IProductScoreService productScoreService,
    IUnitOfWork unitOfWork,
    ILogger<ProductCommentService> logger,
    IMapper mapper
) : ServiceBase(authorizationService, userProvider), IProductCommentService
{
    public async Task<CommentDto> AddCommentForProduct(int productId, ProductCommentCreationDto commentCreationDto)
    {
        var user = await GetUser();

        using var trx = await unitOfWork.StartTransaction();

        try
        {
            var product = await productService.GetProductById(productId);

            if (product is null)
            {
                throw new NotFoundException($"No product with id {productId} found");
            }

            var userComment = await unitOfWork.CommentRepository.GetCommentOfUserForProduct(productId, user.Id);

            if (userComment is not null)
            {
                throw new Exceptions.ApplicationException(
                    "Comment repeation",
                    $"You already registered a comment for product with id {productId}.\nIf you changed your mind edit your preceding comment");
            }

            var rate = await productScoreService.AddScoreForProduct(
                new()
                {
                    ProductId = productId,
                    Score = commentCreationDto.Rate
                }
            );

            var comment = await commentService.CreateComment(
                  new()
                  {
                      ProductId = productId,
                      Text = commentCreationDto.Text,
                      RateId = rate.ProductScoreId
                  }
              );


            await trx.Commit();

            comment.Rate = rate;
            var commentDto = mapper.Map<CommentDto>(comment);

            return commentDto;
        }
        catch (Exceptions.ApplicationException)
        {
            await trx.Rollback();
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "unhandle exception");

            await trx.Rollback();
            throw new Exceptions.ApplicationException("Registering problem", "Could not register comment");
        }


    }

    private async Task<ProductDto> GetProduct(int productId)
    {
        var product = await productService.GetProductById(productId);

        if (product is null)
        {
            throw new NotFoundException($"No product with id {productId} found");
        }

        return product;
    }

    public async Task RemoveCommentOfProduct(int productId, int commentId)
    {


        var product = await productService.GetProductById(productId);

        if (product is null)
        {
            throw new NotFoundException($"No product with id {productId} found");
        }

        var comment = await commentService.GetCommentById(commentId);

        if (comment is null)
        {
            throw new NotFoundException($"No comment with id {commentId} found");
        }

        if (comment.ProductId != productId)
        {
            throw new Exceptions.ApplicationException("Bad inputs", "comment not belong to specified product");
        }

        var author = await GetUser();

        if (comment.User!.Id != author.Id)
        {
            Unauthorized();
            return;
        }

        await commentService.RemoveComment(commentId);

    }

    public Task<CommentDto> UpdateCommentOfProduct(int productId, int CommentId)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResultDto<CommentDto>> GetProductComments(int productId, BaseQuery query)
    {
        var product = await productService.GetProductById(productId);
        if (product is null)
        {
            throw new NotFoundException($"product with id {productId} not found");
        }

        var user = await GetUserOrDefault();
        var comments = await unitOfWork.CommentRepository.GetCommentsOfProduct(productId, query, user?.Id);

        // prepending comments with users comments
        if (user is not null && query.GetPage() == 0)
        {
            var userComments = await unitOfWork.CommentRepository.GetCommentOfUserForProduct(productId, user.Id);

            if (userComments is not null)
            {
                comments.Items = comments.Items.Prepend(userComments);
            }
        }

        return comments;
    }
}
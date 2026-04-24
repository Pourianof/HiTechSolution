using AutoMapper;

using HiTechStore.Core.Auth;
using HiTechStore.Core.Dto.Comment;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Services.Authorization;

namespace HiTechStore.Core.Services.Comment;

public class CommentService : ServiceBase, ICommentService
{
    private IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    public CommentService(IUnitOfWork unitOfWork, IMapper mapper, IAuthorizationService authorizationService, ICurrentUserProvider currentUserProvider) : base(authorizationService, currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Models.Comment> CreateComment(CommentCreationDto commentDto)
    {
        var commentText = commentDto.Text?.Trim();
        if (commentText is null)
        {
            throw new ModelException(
                "Comment text",
                $"Comment must have a text to describe your opinion about product",
                nameof(CommentCreationDto.Text)
            );
        }

        if (commentText.Length <= 3)
        {
            throw new ModelException(
                "Comment text",
                $"Comment must be long enough(at least three character)",
                nameof(CommentCreationDto.Text)
            );
        }
        var user = await GetUser();


        Models.Comment comment;
        if (commentDto.ParentId is not null)
        {
            var parentCommentId = commentDto.ParentId.Value;
            // sub-comment
            var parentComment = await _unitOfWork.CommentRepository.GetModelByIdAsync(parentCommentId);

            if (parentComment is null)
            {
                throw new NotFoundException($"Parent comment with id {parentCommentId} not found");
            }

            comment = _mapper.Map<Models.Comment>(commentDto);
            comment.RateId = null;
            comment.ProductId = null;
            comment.UserId = user.Id!;

        }
        else if (commentDto.ProductId is not null)
        {
            // product-comment
            var product = await _unitOfWork.Products.GetModelByIdAsync(commentDto.ProductId.Value);
            if (product is null)
            {
                throw new NotFoundException("Product owner of comment not found");
            }


            comment = new Models.Comment()
            {
                RateId = commentDto.RateId,
                Text = commentText,
                UserId = user.Id!,
                Product = product
            };
        }
        else
        {
            throw new ModelException(
                "Comment bad data",
                "neither product-id nor parent comment id specified",
                nameof(Models.Comment.ProductId)
            );
        }


        await _unitOfWork.CommentRepository.AddAsync(comment);
        await _unitOfWork.Complete();

        return comment;
    }

    public Task<CommentDto?> GetCommentById(int id)
    {
        return _unitOfWork.CommentRepository.GetByIdProjectedAsync(id);
    }

    public async Task RemoveComment(int id)
    {
        await _unitOfWork.CommentRepository.Delete(id);
        await _unitOfWork.Complete();
    }
}
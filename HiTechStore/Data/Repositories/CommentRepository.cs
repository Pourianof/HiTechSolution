using System.Linq.Expressions;

using AutoMapper;

using HiTechStore.Core.Dto.Comment;
using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs;
using HiTechStore.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories;

public class CommentRepository : Repository<Comment, CommentDto>, ICommentRepository
{
    public CommentRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override IQueryable<CommentDto> HandleProject(IQueryable<Comment> queryable, BaseQuery? query = default)
    {
        return queryable.Select(
             cmnt => new CommentDto
             {
                 CommentId = cmnt.CommentId,
                 ProductId = cmnt.ProductId,
                 Text = cmnt.Text,
                 User = new()
                 {
                     FirstName = cmnt.User!.FirstName,
                     LastName = cmnt.User.LastName
                 },
                 ResponsesCount = cmnt.Responses!.Count(),
                 Rate = cmnt.Rate!.Score,
                 Responses = cmnt.Responses!.OrderByDescending(c => c.CreatedAt).Take(5).Select(
                     cmnt => new CommentDto
                     {
                         CommentId = cmnt.CommentId,
                         ProductId = cmnt.ProductId,
                         Text = cmnt.Text,
                         User = new()
                         {
                             FirstName = cmnt.User!.FirstName,
                             LastName = cmnt.User.LastName
                         },
                         ResponsesCount = cmnt.Responses!.Count()
                     }
                 )
             }
         );
    }

    protected override IQueryable<Comment> GetAllQueryBuilder(IQueryable<Comment> queryBuilder, BaseQuery? queyParams = null)
    {
        var sortBy = queyParams?.SortBy?.GetValue<string>(QueryOperator.Equal);
        if (sortBy is not null)
        {
            Expression<Func<Comment, object>> sorter = sortBy switch
            {
                "created_at" => (Comment cmnt) => cmnt.CreatedAt,
                "reponses" => cmnt => cmnt.Responses!.Count(),
                _ => (Comment cmnt) => cmnt.CreatedAt
            };
            queryBuilder = queryBuilder.OrderBy(sorter);
        }

        return queryBuilder;
    }

    // there is some challenges in querying comments which include the current user possible
    // comment for product. because:
    // 1- sorting: if we wanna place the user's comment in first place, sorting is a challenge
    // 2- paging: in other pages it seems to not include the user's comments because it done in first page
    // 3- limiting: if sorting doesn't obey, then it possible to trim the user's comment from result by limiting
    public Task<PagedResultDto<CommentDto>> GetCommentsOfProduct(int productId, BaseQuery? query, string? userId = default)
    {

        var baseQuery = _dbSet.Where(
            cmnt => cmnt.ProductId == productId && cmnt.UserId != userId
        );

        // if (userId is not null)
        // {
        //     baseQuery =
        //         _dbSet.Where(
        //             cmnt => cmnt.ProductId == productId && cmnt.UserId == userId && cmnt.ParentId == null
        //         ).Union(
        //             baseQuery.Where(cmnt => cmnt.UserId != userId)
        //         );
        // }

        return GetPagedResult<CommentDto>(baseQuery, query);
    }

    public Task<CommentDto?> GetCommentOfUserForProduct(int productId, string userId)
    {
        return Project(
            _dbSet.Where(
                cmnt => cmnt.ProductId == productId && cmnt.UserId == userId
            )
        ).FirstOrDefaultAsync();
    }
}
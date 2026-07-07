
using System.Linq.Expressions;

using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.DTOs.Discount;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Helpers.URLFilterQuery;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories;

using DiscountSorter = Expression<Func<Discount, dynamic>>;

public class DiscountCodeRepository : RepositoryWithIntegerId<Discount, DiscountDto, DiscountQuery>, IDiscountCodeRepository
{
    public DiscountCodeRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<IEnumerable<Discount>> GetActiveDiscountsOfTypeAsync(DiscountType discountType = DiscountType.All)
    {
        var now = DateTime.UtcNow;
        var query = _dbSet.Where(
            (discount) =>
                !discount.IsDeactivated &&
                discount.StartTime < now &&
                discount.EndTime > now
        );

        if (discountType == DiscountType.Codes)
        {
            query = query.Where(d => d.IsDiscountCode);
        }
        else if (discountType == DiscountType.Products)
        {
            query = query.Where(d => !d.IsDiscountCode);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Discount?>> GetDiscountCodeByNameAsync(string name)
    {
        return await _dbSet.Where(
            (code) => EF.Functions.ILike(name, code.Code!)
        ).ToListAsync();
    }

    public async Task<IEnumerable<DiscountDto?>> GetDiscountCodeByNameProjectedAsync(string name)
    {
        return await Project(
            _dbSet.Where(
                (code) => EF.Functions.ILike(name, code.Code!)
            )
        ).ToListAsync();
    }

    protected override IQueryable<Discount> GetAllQueryBuilder(IQueryable<Discount> queryBuilder, DiscountQuery? queryParams = null)
    {
        if (queryParams is null)
        {
            return queryBuilder;
        }

        switch (queryParams.DiscountType)
        {
            case DiscountType.Codes:
                {
                    queryBuilder = queryBuilder.Where(d => d.IsDiscountCode);
                    break;
                }
            case DiscountType.Products:
                {
                    queryBuilder = queryBuilder.Where(d => !d.IsDiscountCode);
                    break;
                }
        }


        if (queryParams.SortBy is not null)
        {
            var sortBy = queryParams.SortBy.GetValue<string>(QueryOperator.Equal);

            if (sortBy is not null)
            {
                var sortyByCriterias = sortBy.Split(",").Select(sb => sb.Trim().ToLower());

                var hasOrdered = false;

                foreach (var criteria in sortyByCriterias)
                {
                    DiscountSorter sorter = criteria switch
                    {
                        "endtime" => (dc) => dc.EndTime!,
                        "startime" => (dc) => dc.StartTime!,
                        "id" => (dc) => dc.DiscountId,
                        _ => (dc) => dc.CreatedAt
                    };
                    queryBuilder = hasOrdered ?
                        (queryBuilder as IOrderedQueryable<Discount>)!.ThenBy(sorter) :
                        queryBuilder.OrderBy(sorter);

                    hasOrdered = true;
                }
            }

        }

        return queryBuilder;
    }
}


using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.DiscountCode;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories;

public class DiscountCodeRepository : Repository<DiscountCode, DiscountCodeDto>, IDiscountCodeRepository
{
    public DiscountCodeRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public Task<DiscountCode?> GetDiscountCodeByNameAsync(string name)
    {
        return _dbSet.FirstOrDefaultAsync(
            (code) => EF.Functions.ILike(name, code.Code!)
        );
    }

    public Task<DiscountCodeDto?> GetDiscountCodeByNameProjectedAsync(string name)
    {
        return Project(
            _dbSet.Where(
                (code) => EF.Functions.ILike(name, code.Code!)
            )
        ).FirstOrDefaultAsync();
    }

    protected override IQueryable<DiscountCode> GetAllQueryBuilder(IQueryable<DiscountCode> queryBuilder, BaseQuery? queryParams = null)
    {
        if (queryParams is null)
        {
            return queryBuilder;
        }


        if (queryParams.SortBy is not null)
        {
            var sortBy = queryParams.SortBy.GetValue<string>(Helpers.URLFilterQuery.QueryOperator.Equal);

            if (sortBy is not null)
            {
                var sortyByCriterias = sortBy.Split(",").Select(sb => sb.Trim().ToLower());

                foreach (var criteria in sortyByCriterias)
                {
                    switch (criteria)
                    {
                        case "endtime":
                            queryBuilder = queryBuilder.OrderBy((dc) => dc.EndTime);
                            break;
                        case "startime":
                            queryBuilder = queryBuilder.OrderBy((dc) => dc.StartTime);
                            break;
                        case "id":
                            queryBuilder = queryBuilder.OrderBy((dc) => dc.DiscountCodeId);
                            break;
                    }
                }
            }

        }

        return queryBuilder;
    }
}

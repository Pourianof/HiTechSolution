
using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.Discount;
using HiTechStore.Data.Queries;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Data.Repositories;

public class DiscountCodeRepository : Repository<Discount, DiscountDto>, IDiscountCodeRepository
{
    public DiscountCodeRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<IEnumerable<Discount>> GetActiveDiscountsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet.Where(
            (discount) =>
                !discount.IsDeactivated &&
                discount.StartTime < now &&
                discount.EndTime > now
        ).ToListAsync();
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

    protected override IQueryable<Discount> GetAllQueryBuilder(IQueryable<Discount> queryBuilder, BaseQuery? queryParams = null)
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
                            queryBuilder = queryBuilder.OrderBy((dc) => dc.DiscountId);
                            break;
                    }
                }
            }

        }

        return queryBuilder;
    }
}

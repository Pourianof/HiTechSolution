

using AutoMapper;

using HiTechStore.Core.Repositories;
using HiTechStore.Data.DTOs.DiscountEntity;
using HiTechStore.Models;

using Microsoft.EntityFrameworkCore;


namespace HiTechStore.Data.Repositories;

public class DiscountEntityRepository : Repository<DiscountEntity, DiscountEntityDto>, IDiscountEntityRepository
{
    public DiscountEntityRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }


    private async Task<DiscountEntity> AddEntitySafeAsync(DiscountEntity discountEntity)
    {
        var dbEntity = await _dbSet.FirstOrDefaultAsync(e => e.Name == discountEntity.Name);

        var entityProperties = discountEntity.Properties;
        discountEntity.Properties = [];

        if (dbEntity is null)
        {
            await AddAsync(discountEntity);
            await _context.SaveChangesAsync();
        }
        else
        {
            discountEntity = dbEntity;
        }


        if (entityProperties is null || !entityProperties.Any())
        {
            return discountEntity;
        }

        foreach (var entityProperty in entityProperties)
        {
            var dbEntityProperty = await _context.Set<DiscountEntityProperty>().FirstOrDefaultAsync(ep => EF.Functions.ILike(ep.Path!, entityProperty.Path!));
            var propertySubEntity = entityProperty.SubEntity;
            entityProperty.SubEntity = null;

            if (dbEntityProperty is null)
            {
                entityProperty.Entity = discountEntity;
                await _context.AddAsync(entityProperty);
                await _context.SaveChangesAsync();
            }

            if (propertySubEntity is not null)
            {
                var entity = await AddEntitySafeAsync(propertySubEntity);
                entityProperty.SubEntity = entity;
                await _context.SaveChangesAsync();
            }
        }
        return discountEntity;
    }

    public async Task AddAllSafeAsync(IEnumerable<DiscountEntity> discountEntities)
    {
        // await Task.Delay(TimeSpan.FromSeconds(30));

        foreach (var entity in discountEntities)
        {
            await AddEntitySafeAsync(entity);
        }
    }

    public async Task<DiscountEntityProperty?> GetPropertyById(int propertyId)
    {
        return await _context.Set<DiscountEntityProperty>().FirstOrDefaultAsync(
            (p) => p.DiscountEntityPropertyId == propertyId
        );
    }
}

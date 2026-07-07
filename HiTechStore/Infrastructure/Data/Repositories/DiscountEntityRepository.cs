

using AutoMapper;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.DTOs.DiscountEntity;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;


namespace HiTechStore.Infrastructure.Data.Repositories;

public class DiscountEntityRepository : RepositoryWithIntegerId<DiscountEntity, DiscountEntityDto>, IDiscountEntityRepository
{
    public DiscountEntityRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }



    public async Task AddAllSafeAsync(IEnumerable<DiscountEntity> discountEntities)
    {
        var registerer = new DiscountEntitySafelyRegisterer(this, _context);
        await registerer.AddAllSafeAsync(discountEntities);
        await _context.SaveChangesAsync();

    }

    public async Task<DiscountEntityProperty?> GetPropertyById(int propertyId)
    {
        return await _context.Set<DiscountEntityProperty>().FirstOrDefaultAsync(
            (p) => p.DiscountEntityPropertyId == propertyId
        );
    }

    public async Task<DiscountEntityProperty?> GetPropertyByEntityAsync(string entityName, string propertyName)
    {
        return await _context.Set<DiscountEntityProperty>().FirstOrDefaultAsync(ep => ep.Name == propertyName && ep.Entity!.Name == entityName);
    }

    public async Task<ConditionMethod?> GetConditionMethodByNameAsync(string methodName)
    {
        return await _context.Set<ConditionMethod>().FirstOrDefaultAsync(
            m => EF.Functions.ILike(m.Name!, methodName)
        );
    }
}

// A class which has guard to don't stuck in a property->entity->property infinite cycle
class DiscountEntitySafelyRegisterer(IDiscountEntityRepository Repo, HiTechStoreDbContext Context)
{
    private Dictionary<string, DiscountEntity> ReachedEntities { get; set; } = new();
    private async Task<DiscountEntity> AddEntitySafeAsync(DiscountEntity discountEntity)
    {
        var dbEntity = await Context.Set<DiscountEntity>().FirstOrDefaultAsync(e => e.Name == discountEntity.Name);

        if (ReachedEntities.ContainsKey(discountEntity.Name!))
        {
            return ReachedEntities[discountEntity.Name!];
        }

        if (dbEntity is null)
        {
            await Repo.AddAsync(discountEntity);
            dbEntity = discountEntity;
        }
        ReachedEntities.Add(dbEntity.Name!, dbEntity);

        var entityProperties = new List<DiscountEntityProperty>(discountEntity.Properties ?? []);
        dbEntity.Properties ??= new List<DiscountEntityProperty>();

        if (!entityProperties.Any())
        {
            return dbEntity;
        }

        foreach (var entityProperty in entityProperties)
        {
            var dbEntityProperty = await Repo.GetPropertyByEntityAsync(dbEntity!.Name!, entityProperty.Name!);
            var propertySubEntity = entityProperty.SubEntity;


            if (propertySubEntity is not null)
            {
                var entity = await AddEntitySafeAsync(propertySubEntity);
                entityProperty.SubEntity = entity;
            }


            if (dbEntityProperty is null)
            {
                dbEntity.Properties.Add(entityProperty);
            }
        }
        return dbEntity;
    }

    public async Task AddAllSafeAsync(IEnumerable<DiscountEntity> discountEntities)
    {
        foreach (var entity in discountEntities)
        {
            await AddEntitySafeAsync(entity);
        }
    }
}
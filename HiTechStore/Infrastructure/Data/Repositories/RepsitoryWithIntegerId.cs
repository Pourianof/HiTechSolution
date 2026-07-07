
using AutoMapper;

using HiTechStore.Core;
using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Infrastructure.Data.Queries;


namespace HiTechStore.Infrastructure.Data.Repositories;

public class RepositoryCoreWithIntegerId<TModel>(HiTechStoreDbContext context) : RepositoryCore<TModel, int>(context)
    where TModel : class, IModel
{ }
public class RepositoryWithIntegerId<TModel, TProject, TQuery>(HiTechStoreDbContext context, IMapper mapper) :
    Repository<TModel, TProject, TQuery, int>(context, mapper), IRepositoryWithIntegerId<TModel, TProject, TQuery>
        where TModel : class, IModel
        where TQuery : BaseQuery
        where TProject : class
{ }

public class RepositoryWithIntegerId<TModel, TProject>(HiTechStoreDbContext context, IMapper mapper)
    : Repository<TModel, TProject, BaseQuery, int>(context, mapper)
      where TModel : class, IModel
      where TProject : class
{ }

public class RepositoryWithIntegerId<TModel> : Repository<TModel, TModel, BaseQuery, int>
        where TModel : class, IModel
{
    public RepositoryWithIntegerId(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }
}

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public class ResourceExistenceResult<TId>
    where TId : struct
{
    public TId Id { get; set; }
    public bool DoesExist { get; set; }
}

public class ResourceExistenceResultWithModel<TModel, TId> : ResourceExistenceResult<TId>
    where TModel : class, IModel
    where TId : struct
{
    public TModel? Model;
}

public class ResourceExistenceResultWithIntegerIdModel<TModel> : ResourceExistenceResult<int>
    where TModel : class, IModel
{
}
namespace HiTechStore.Core.Repositories;

public class ResourceExistenceResult
{
    public int Id { get; set; }
    public bool DoesExist { get; set; }
}

public class ResourceExistenceResultWithModel<TModel> : ResourceExistenceResult
    where TModel : class, IModel
{
    public TModel? Model;
}
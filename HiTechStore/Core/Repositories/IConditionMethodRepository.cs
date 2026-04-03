using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IConditionMethodRepository : IRepository<ConditionMethod>
{
    Task AddAllSafe(IEnumerable<ConditionMethod> conditionMethods);
}
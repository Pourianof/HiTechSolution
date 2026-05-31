using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IConditionMethodRepository : IRepository<ConditionMethod>
{
    Task AddAllSafe(IEnumerable<ConditionMethod> conditionMethods);
}
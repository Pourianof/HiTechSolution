using HiTechStore.Core.Models;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;

public interface IColorRepository : IRepository<Color>
{
    Task<Color?> GetColorByNameAsync(string name);
}
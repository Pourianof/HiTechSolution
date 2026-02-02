using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;

public interface IColorRepository : IRepository<Color>
{
    Task<Color?> GetColorByNameAsync(string name);
}
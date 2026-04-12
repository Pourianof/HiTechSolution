using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;


public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(string id);
}
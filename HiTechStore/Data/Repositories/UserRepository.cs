
using HiTechStore.Core.Repositories;
using HiTechStore.Models;

using Microsoft.AspNetCore.Identity;

namespace HiTechStore.Data.Repositories;

public class UserRepository(UserManager<User> userManager) : IUserRepository
{
    public Task<User?> GetUserByIdAsync(string id)
    {
        return userManager.FindByIdAsync(id);
    }
}
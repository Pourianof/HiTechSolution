
using System.Security.Claims;

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

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        return userManager.FindByIdAsync(username);
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        return userManager.FindByIdAsync(email);
    }

    public Task<bool> CheckUserPasswordAsync(User user, string password)
    {
        return userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IEnumerable<Claim>> GetUserClaims(User user)
    {
        return (await userManager.GetClaimsAsync(user)).AsEnumerable();
    }

    public async Task AddClaimUser(User user, IEnumerable<Claim> claims)
    {
        await userManager.AddClaimsAsync(user, claims);
    }

}
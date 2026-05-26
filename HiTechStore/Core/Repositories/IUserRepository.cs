using System.Security.Claims;

using HiTechStore.Core.Helpers.Result;
using HiTechStore.Models;

namespace HiTechStore.Core.Repositories;


public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(string id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> CheckUserPasswordAsync(User user, string password);
    Task<IEnumerable<Claim>> GetUserClaims(User user);
    Task AddClaimUser(User user, IEnumerable<Claim> claims);
    Task<IEnumerable<string>> GetUserRoles(User user);
    Task<bool> UpdateUser(User user);
    Task<Result<bool>> ChangePassword(User user, string oldPassowrd, string newPassword);
    Task<string> GenerateChangePasswordToken(User user);
    Task<Result<bool>> ResetPasswordByToken(User user, string token, string newPassword);
}
using System.Security.Claims;

using HiTechStore.Core.Dto.Auth;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Core.Models;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.Queries;

namespace HiTechStore.Core.Common.Interfaces.Infra.Repositories;


public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(string id);
    Task<UserDto?> GetUserDtoByIdAsync(string id);
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
    Task<Result<bool>> RegisterUser(User user, string password);
    Task<Result<bool>> AddRoleToUser(User user, string role);
    Task<Result<bool>> DeleteUser(User user);
    Task<Result<bool>> CheckUsernameExists(string username);
    Task<Result<PagedResultDto<UserDto>>> GetUsers(UserQuery userQuery);
}
using System.Security.Claims;

using HiTechStore.Core.Dto.Auth;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Models;

namespace HiTechStore.Core.Services.Authorization;

public interface IAuthorizationService
{
    Task<User?> LoginAsync(LoginDto loginDto);
    Task<User?> GetUserAsync(IEnumerable<Claim> claims);
    Task<User?> GetUserByIdAsync(string userId);
    Task<Result<bool>> ChangePassword(ChangePasswordDto changePasswordDto);
    Task RequestPasswordResetAsync(string email, Func<string, string> accessPointProvider);
    Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword);
}
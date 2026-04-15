using System.Security.Claims;

using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Models;

namespace HiTechStore.Core.Services.Authorization;

public interface IAuthorizationService
{
    Task<User?> LoginAsync(LoginDto loginDto);
    Task<User?> GetUserAsync(IEnumerable<Claim> claims);
}
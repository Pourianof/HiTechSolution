using System.Security.Claims;

using HiTechStore.Core.Exceptions;
using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Models;

namespace HiTechStore.Core.Services.Authorization;


public class AuthorizationService(IUnitOfWork unitOfWork) : IAuthorizationService
{
    public async Task<User?> LoginAsync(LoginDto loginDto)
    {

        if (loginDto.Email is null && loginDto.Username is null)
        {
            throw new ModelException("No user identifier", "No user identifier(either email or username) specified", nameof(LoginDto.Username));
        }

        User? user;
        if (loginDto.Email is not null)
        {
            user = await unitOfWork.UserRepository.GetUserByEmailAsync(loginDto.Email!);
        }
        else
        {
            user = await unitOfWork.UserRepository.GetUserByUsernameAsync(loginDto.Username!);
        }

        if (user == null || !await unitOfWork.UserRepository.CheckUserPasswordAsync(user, loginDto.Password!))
        {
            return null;
        }

        var roles = user.Roles?.Select(r => r.Name) ?? [IdentityRoles.User];

        user.Claims = ProvideUserClaims(user, roles!);

        return user;

    }

    public async Task<User?> GetUserAsync(IEnumerable<Claim> claims)
    {
        var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (userId is not null)
        {
            return await unitOfWork.UserRepository.GetUserByIdAsync(userId);
        }

        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email is not null)
        {
            return await unitOfWork.UserRepository.GetUserByEmailAsync(email);
        }

        var username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        if (username is not null)
        {
            return await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        }

        throw new ModelException("No user identifier", "No user identifier specified for identifiying it", "username");
    }

    private IEnumerable<Claim> ProvideUserClaims(User user, IEnumerable<string> roles)
    {
        return [
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Role, string.Join(",", roles)),
            new Claim(ClaimTypes.Email, user.Email!),
        ];
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        var user = await unitOfWork.UserRepository.GetUserByIdAsync(userId);

        if (user is null)
        {
            return default;
        }

        var roles = user.Roles?.Select(r => r.Name!) ?? [IdentityRoles.User];
        user.Claims = ProvideUserClaims(user, roles);

        return user;
    }
}


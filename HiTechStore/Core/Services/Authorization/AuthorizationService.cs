using System.Security.Claims;

using HiTechStore.Core.Auth;
using HiTechStore.Core.Dto.Auth;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Data.DTOs.Authorization;
using HiTechStore.Models;

namespace HiTechStore.Core.Services.Authorization;


public class AuthorizationService(IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider) : IAuthorizationService
{

    private async Task<User?> EnrichUserWithClaimsAndRoles(User user)
    {
        var roles = await unitOfWork.UserRepository.GetUserRoles(user);

        if (!roles.Any())
        {
            roles = [IdentityRoles.User];
        }

        user.Claims = ProvideUserClaims(user, roles!);
        user.Roles = roles;

        return user;
    }
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

        user = await EnrichUserWithClaimsAndRoles(user);

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

        user = await EnrichUserWithClaimsAndRoles(user);

        return user;
    }

    public async Task<Result<bool>> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var validationErrors = new List<ResultError>();

        if (string.IsNullOrWhiteSpace(changePasswordDto.OldPassword))
        {
            validationErrors.Add(AuthorizationErrors.OldPasswordRequired());
        }

        if (string.IsNullOrWhiteSpace(changePasswordDto.NewPassword))
        {
            validationErrors.Add(AuthorizationErrors.NewPasswordRequired());
        }

        if (string.IsNullOrWhiteSpace(changePasswordDto.PasswordConfirmation))
        {
            validationErrors.Add(AuthorizationErrors.PasswordConfirmationRequired());
        }
        else if (changePasswordDto.NewPassword != changePasswordDto.PasswordConfirmation)
        {
            validationErrors.Add(AuthorizationErrors.PasswordConfirmationMismatch());
        }

        if (validationErrors.Any())
        {
            return new Result<bool>
            {
                Value = false,
                Errors = validationErrors
            };
        }

        if (!currentUserProvider.IsAuthorized)
        {
            throw new NotAllowedException();
        }

        var user = await GetUserByIdAsync(currentUserProvider.UserId!);

        if (user is null)
        {
            throw new NotAllowedException();
        }

        var result = await unitOfWork.UserRepository.ChangePassword(user, changePasswordDto.OldPassword!, changePasswordDto.NewPassword!);

        return result;
    }
}


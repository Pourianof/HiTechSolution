
using System.Security.Claims;

using HiTechStore.Core.Dto.Auth;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Core.Models;

using Microsoft.AspNetCore.Identity;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Infrastructure.Data.Repositories.Helpers;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;
using HiTechStore.Helpers.URLFilterQuery;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class UserRepository(
    UserManager<User> userManager,
    HiTechStoreDbContext dbContext,
    IMapper mapper
    ) : IUserRepository
{
    public Task<User?> GetUserByIdAsync(string id)
    {
        return userManager.FindByIdAsync(id);
    }

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        return userManager.FindByNameAsync(username);
    }

    public Task<User?> GetUserByEmailAsync(string email)
    {
        return userManager.FindByEmailAsync(email);
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

    public async Task<IEnumerable<string>> GetUserRoles(User user)
    {
        return await userManager.GetRolesAsync(user);
    }

    public async Task<bool> UpdateUser(User user)
    {
        return (await userManager.UpdateAsync(user)).Succeeded;
    }

    public async Task<Result<bool>> ChangePassword(User user, string oldPassowrd, string newPassword)
    {
        var result = await userManager.ChangePasswordAsync(user, oldPassowrd, newPassword);

        return new()
        {
            Value = result.Succeeded,
            Errors = result.Errors.MapToResultError()
        };
    }

    public async Task<string> GenerateChangePasswordToken(User user)
    {
        return await userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<Result<bool>> ResetPasswordByToken(User user, string token, string newPassword)
    {
        var result = await userManager.ResetPasswordAsync(user, token, newPassword);

        return new Result<bool>
        {
            Value = result.Succeeded,
            Errors = result.Errors.MapToResultError()
        };
    }

    public async Task<Result<bool>> RegisterUser(User user, string password)
    {
        var result = await userManager.CreateAsync(user, password);

        return new Result<bool>
        {
            Value = result.Succeeded,
            Errors = result.Errors.MapToResultError()
        };
    }

    public async Task<Result<bool>> AddRoleToUser(User user, string role)
    {
        var res = await userManager.AddToRoleAsync(user, role);
        return new()
        {
            Value = res.Succeeded,
        };
    }

    public async Task<Result<bool>> DeleteUser(User user)
    {
        var res = await userManager.DeleteAsync(user);

        return new()
        {
            Value = res.Succeeded,
        };
    }

    public async Task<Result<bool>> CheckUsernameExists(string username)
    {
        var user = await userManager.FindByNameAsync(username);

        return new()
        {
            Value = user is not null,
        };
    }

    public async Task<Result<PagedResultDto<UserDto>>> GetUsers(UserQuery userQuery)
    {
        IQueryable<User> query = dbContext.Users.AsQueryable();

        var username = userQuery.Username?.GetValue<string>(QueryOperator.Equal)?.Trim();
        if (!string.IsNullOrEmpty(username))
        {
            query = query.Where(
                user => EF.Functions.ILike(
                    user.UserName!, $"%{username}%"
                )
            );
        }

        var finalResult = RepositoryHelper.BuildQueryBuilderBasedOnQueryParams(
            query,
            userQuery
        );

        return new()
        {
            Value = new PagedResultDto<UserDto>()
            {
                Items = finalResult.AppliedQuery.ProjectTo<UserDto>(mapper.ConfigurationProvider),// await Project<TOut>(query.AppliedQuery!, queryParams).ToListAsync(),
                PageNumber = finalResult.Page,
                PageSize = finalResult.PageSize,
                TotalCount = await finalResult.BaseQuery!.CountAsync()
            }
        };
    }
}

public static class IdentityErrorExtension
{
    public static List<ResultError> MapToResultError(this IEnumerable<IdentityError> errors)
    {
        return errors.Select(ie => ie.Code switch
        {
            "PasswordMismatch" => (ResultError)Core.Services.Authorization.AuthErrors.PasswordMismatch(ie.Description),
            "PasswordRequiresDigit" => Core.Services.Authorization.AuthErrors.PasswordRequiresDigit(),
            "PasswordRequiresLower" => Core.Services.Authorization.AuthErrors.PasswordRequiresLower(),
            "PasswordRequiresUpper" => Core.Services.Authorization.AuthErrors.PasswordRequiresUpper(),
            "PasswordRequiresNonAlphanumeric" => Core.Services.Authorization.AuthErrors.PasswordRequiresNonAlphanumeric(),
            "PasswordTooShort" => Core.Services.Authorization.AuthErrors.PasswordTooShort(ie.Description),
            _ => Core.Services.Authorization.AuthErrors.GenericPassword(ie.Description ?? ie.Code, ie.Description, ie.Code, nameof(ChangePasswordDto.NewPassword))
        }).ToList();

    }
}
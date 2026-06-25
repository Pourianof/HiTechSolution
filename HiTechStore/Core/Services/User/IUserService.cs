using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Dto.Auth;
using HiTechStore.Core.Helpers.Result;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Infrastructure.Data.Queries;

namespace HiTechStore.Core.Services.UserService; // choose name UserService to avoid conflict with User model name

public interface IUserService
{
    Task<string> UpdateProfileAvatar(AppFile avatar);
    Task<Result<PagedResultDto<UserDto>>> GetUsers(UserQuery query);
}


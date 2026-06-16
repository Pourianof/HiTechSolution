using HiTechStore.Core.Common.Interfaces.Infra;

namespace HiTechStore.Core.Services.UserService; // choose name UserService to avoid conflict with User model name

public interface IUserService
{
    Task<string> UpdateProfileAvatar(AppFile avatar);
}


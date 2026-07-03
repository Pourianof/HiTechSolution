

using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Exceptions;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Core.Models;
using HiTechStore.Infrastructure.Data.DTOs;
using HiTechStore.Core.Dto.Auth;
using HiTechStore.Infrastructure.Data.Queries;
using HiTechStore.Core.Helpers.Result;

namespace HiTechStore.Core.Services.UserService;

public class UserService : ServiceBase, IUserService
{
    private IPublicAssetRegisterer _assetRegisterer;
    private IUnitOfWork _unitOfWork;
    private UsersServicePermissionHelper _usersServicePermissionHelper;
    public UserService(
        IAuthorizationService authorizationService,
        ICurrentUserProvider currentUserProvider,
        IPublicAssetRegisterer assetRegisterer,
        IUnitOfWork unitOfWork,
        UsersServicePermissionHelper usersServicePermissionHelper
    ) : base(authorizationService, currentUserProvider)
    {
        _assetRegisterer = assetRegisterer;
        _unitOfWork = unitOfWork;
        _usersServicePermissionHelper = usersServicePermissionHelper;
    }

    public async Task<string> UpdateProfileAvatar(AppFile avatar)
    {
        var isImage = MediaTypeHelper.IsImage(avatar.FileName);

        if (!isImage)
        {
            throw new ModelException("Invalid data", $"specified file is not valid image with one of these formats: {MediaTypeHelper.GetValidImageTypes()}", nameof(avatar));
        }


        var user = await GetUser();

        string? filePath = default;

        try
        {
            filePath = await _assetRegisterer.SaveFileAsync(avatar, new WriteFileOptions
            {
                PathParts = ["images", "avatars"],
                WellDistributedPath = true
            });

            var userOldAvatar = user.AvatarUrl;

            user.AvatarUrl = filePath;

            var succeeded = await _unitOfWork.UserRepository.UpdateUser(user);

            if (succeeded && userOldAvatar is not null)
            {
                // maybe should delay this task to background jobs
                _assetRegisterer.DeleteFile(userOldAvatar);
            }
            return _assetRegisterer.GetPublicUrl(filePath);
        }
        catch
        {
            if (filePath is not null)
            {
                _assetRegisterer.DeleteFile(filePath);
            }

            throw;
        }
    }

    public async Task<Result<PagedResultDto<UserDto>>> GetUsers(UserQuery query)
    {
        if (await _usersServicePermissionHelper.HasPermissionToGetUsersList(UserIdOrThrow))
        {
            return await _unitOfWork.UserRepository.GetUsers(query);
        }

        throw new NotAllowedException(
            "Not allowed",
            "You have not permission to access this service"
        );

    }
}

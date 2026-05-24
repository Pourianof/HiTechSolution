

using HiTechStore.Core.Auth;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Services.Authorization;

namespace HiTechStore.Core.Services.UserService;

public class UserService : ServiceBase, IUserService
{
    private IPublicAssetRegisterer _assetRegisterer;
    private IUnitOfWork _unitOfWork;
    public UserService(
        IAuthorizationService authorizationService,
        ICurrentUserProvider currentUserProvider,
        IPublicAssetRegisterer assetRegisterer,
        IUnitOfWork unitOfWork
    ) : base(authorizationService, currentUserProvider)
    {
        _assetRegisterer = assetRegisterer;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> UpdateProfileAvatar(IFormFile avatar)
    {
        var user = await GetUser();

        string? filePath = default;

        try
        {
            filePath = await _assetRegisterer.WriteIFormFile(avatar, new WriteFileOptions
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
            return filePath;
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
}


using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Authorization;

namespace HiTechStore.Core.Services.Notification;

public class NotificationService : ServiceBase, INotificationService
{
    public NotificationService(IAuthorizationService authorizationService, ICurrentUserProvider currentUserProvider) : base(authorizationService, currentUserProvider)
    {
    }

    public Task<UserNotification> CreateNotification()
    {
        throw new NotImplementedException();
    }

    public Task SyncNotifications()
    {
        throw new NotImplementedException();
    }
}
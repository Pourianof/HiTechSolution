


using AutoMapper;

using HiTechStore.Core.Common.Events;
using HiTechStore.Core.Common.Interfaces.Infra;
using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Core.Common.Interfaces.Presentation;
using HiTechStore.Core.Dto.UserNotification;
using HiTechStore.Core.Models;
using HiTechStore.Core.Services.Authorization;
using HiTechStore.Infrastructure.Data.DTOs;

namespace HiTechStore.Core.Services.Notification;

public class NotificationService : ServiceBase, INotificationService
{
    private IUnitOfWork _unitOfWork;
    private IMapper _mapper;
    private IEventPublisher _eventPublisher;
    public NotificationService(
        IAuthorizationService authorizationService,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork,
        IEventPublisher eventPublisher,
        IMapper mapper
    ) : base(authorizationService, currentUserProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }
    public async Task<UserNotification> CreateNotification(CreateNotificationDto notificationDto)
    {
        var notification = _mapper.Map<UserNotification>(notificationDto);
        await _unitOfWork.UserNotificationRepository.AddAsync(notification);

        await _eventPublisher.PublishAsync(
           new UserNotificationCreatedEvent()
           {
               NotificationId = notification.Id
           }
       );

        await _unitOfWork.Complete();

        return notification;
    }

    public Task<PagedResultDto<UserNotificationDto>> GetNotifications(string userId, NotificationQuery query)
    {
        return _unitOfWork.UserNotificationRepository.GetUsersNotifications(userId, query);
    }

    public Task SyncNotifications()
    {
        throw new NotImplementedException();
    }
}
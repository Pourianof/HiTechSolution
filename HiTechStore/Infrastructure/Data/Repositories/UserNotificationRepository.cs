
using AutoMapper;
using AutoMapper.QueryableExtensions;

using HiTechStore.Core.Common.Interfaces.Infra.Repositories;
using HiTechStore.Core.Dto.UserNotification;
using HiTechStore.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace HiTechStore.Infrastructure.Data.Repositories;

public class UserNotificationRepository : Repository<UserNotification, UserNotificationDto, Guid>, IUserNotificationRepository
{
    public UserNotificationRepository(HiTechStoreDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    public async Task<IEnumerable<UserNotificationDto>> GetUnreadNotifications(string userId)
    {
        return await _dbSet.Where(
            un => un.ReadAt == null
        )
        .ProjectTo<UserNotificationDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
    }
}
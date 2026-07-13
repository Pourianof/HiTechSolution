using HiTechStore.Core.Common.Events;

namespace HiTechStore.Core.Common.Interfaces.Infra;

public interface IEventPublisher
{
    Task PublishAsync(IEvent @event);
}
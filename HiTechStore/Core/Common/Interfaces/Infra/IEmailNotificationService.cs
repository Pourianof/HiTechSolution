namespace HiTechStore.Core.Common.Interfaces.Infra;

public interface IEmailNotificationService
{
    Task NotifyAsync(EmailNotification notification);
}

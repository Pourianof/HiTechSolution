using HiTechStore.Core.Common.Interfaces.Infra;

namespace HiTechStore.Infrastructure.Email;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateRenderer _templateRenderer;

    public EmailNotificationService(IEmailSender emailSender, IEmailTemplateRenderer templateRenderer)
    {
        _emailSender = emailSender;
        _templateRenderer = templateRenderer;
    }

    public async Task NotifyAsync(EmailNotification notification)
    {
        if (notification is null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        var htmlBody = _templateRenderer.Render(notification.TemplateName, notification.Model);
        await _emailSender.SendAsync(notification.To, notification.Subject, htmlBody).ConfigureAwait(false);
    }
}

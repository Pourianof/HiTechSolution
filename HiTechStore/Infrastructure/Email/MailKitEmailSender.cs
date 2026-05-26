using HiTechStore.Core.Common.Interfaces.Infra;

using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Options;

using MimeKit;

namespace HiTechStore.Infrastructure.Email;

public class MailKitEmailSender : IEmailSender
{
    private readonly EmailSettings settings;

    public MailKitEmailSender(IOptions<EmailSettings> options)
    {
        settings = options.Value;
    }

    public async Task SendAsync(string to, string subject, string htmlBody, string? plainText = null)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            throw new ArgumentException("Email recipient cannot be empty.", nameof(to));
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(settings.FromName, settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = plainText ?? StripHtml(htmlBody)
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        var secureSocketOptions = settings.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.Auto;

        await smtp.ConnectAsync(settings.Host, settings.Port, secureSocketOptions).ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(settings.UserName))
        {
            await smtp.AuthenticateAsync(settings.UserName, settings.Password ?? string.Empty).ConfigureAwait(false);
        }

        await smtp.SendAsync(message).ConfigureAwait(false);
        await smtp.DisconnectAsync(true).ConfigureAwait(false);
    }

    private static string StripHtml(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var array = new char[input.Length];
        var arrayIndex = 0;
        var inside = false;

        foreach (var @let in input)
        {
            if (@let == '<')
            {
                inside = true;
                continue;
            }
            if (@let == '>')
            {
                inside = false;
                continue;
            }
            if (!inside)
            {
                array[arrayIndex++] = @let;
            }
        }

        return new string(array, 0, arrayIndex);
    }
}

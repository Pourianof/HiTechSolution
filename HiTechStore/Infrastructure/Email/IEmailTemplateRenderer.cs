namespace HiTechStore.Infrastructure.Email;

public interface IEmailTemplateRenderer
{
    string Render(string templateName, object? model = null);
}

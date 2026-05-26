using System.Net;
using System.Reflection;
using System.Collections.Concurrent;


namespace HiTechStore.Infrastructure.Email;

public class FileEmailTemplateRenderer : IEmailTemplateRenderer
{
    private readonly string templateFolder;
    private readonly ConcurrentDictionary<string, string> templateCache = new(StringComparer.OrdinalIgnoreCase);

    public FileEmailTemplateRenderer(IHostEnvironment environment)
    {
        templateFolder = Path.Combine(environment.ContentRootPath, "Infrastructure", "Email", "Templates");
    }

    public string Render(string templateName, object? model = null)
    {
        if (string.IsNullOrWhiteSpace(templateName))
        {
            throw new ArgumentException("Template name cannot be empty.", nameof(templateName));
        }

        var template = LoadTemplate(templateName);
        if (model is null)
        {
            return template;
        }

        var properties = model.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name, p => p.GetValue(model)?.ToString() ?? string.Empty, StringComparer.OrdinalIgnoreCase);

        foreach (var (key, value) in properties)
        {
            template = template.Replace($"{{{{{key}}}}}", WebUtility.HtmlEncode(value));
        }

        return template;
    }

    private string LoadTemplate(string templateName)
    {
        return templateCache.GetOrAdd(templateName, name =>
        {
            var fileName = name.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
                ? name
                : name + ".html";
            var fullPath = Path.Combine(templateFolder, fileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Email template '{fileName}' was not found.", fullPath);
            }

            return File.ReadAllText(fullPath);
        });
    }
}

using System.Collections.Concurrent;
using Fluid;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Emails;

public class EmailTemplateService(IWebHostEnvironment? webHostEnvironment, ILogger<EmailTemplateService> logger)
    : IEmailTemplateService
{
    private readonly ConcurrentDictionary<string, string> _templateCache = new(StringComparer.OrdinalIgnoreCase);

    private readonly FluidParser _parser = new();


    public async Task LoadTemplateAsync(string templateKey, string fileName, string defaultTemplate,
        CancellationToken cancellationToken = default)
    {
        var filePath = webHostEnvironment != null
            ? Path.Combine(webHostEnvironment.ContentRootPath, "Templates", fileName)
            : Path.Combine(AppContext.BaseDirectory, "Templates", fileName);

        if (File.Exists(filePath))
        {
            logger.LogInformation("Loading email template '{TemplateKey}' from host path: {FilePath}", templateKey,
                filePath);
            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
            _templateCache[templateKey] = content;
        }
        else
        {
            logger.LogWarning(
                "Host template file not found at {FilePath}. Fallback to default in-memory template for '{TemplateKey}'.",
                filePath, templateKey);
            _templateCache[templateKey] = defaultTemplate;
        }
    }

    public async Task<string> RenderTemplateAsync(string key, Dictionary<string, string> model,
        CancellationToken cancellationToken = default)
    {
        if (!_templateCache.TryGetValue(key, out var templateString))
        {
            throw new InvalidOperationException($"Email template '{key}' not found in cache.");
        }

        if (model.Count == 0)
        {
            return templateString;
        }

        var template = _parser.Parse(templateString);
        var context = new TemplateContext(model);

        return await template.RenderAsync(context);
    }
}
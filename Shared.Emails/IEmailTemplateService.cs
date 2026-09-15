namespace Shared.Emails;

public interface IEmailTemplateService
{
    // Task InitializeAsync(CancellationToken cancellationToken = default);

    public Task LoadTemplateAsync(string templateKey, string fileName, string defaultTemplate,
        CancellationToken cancellationToken= default);
    

    public Task<string> RenderTemplateAsync(string key, Dictionary<string, string> model,CancellationToken cancellationToken = default);
}
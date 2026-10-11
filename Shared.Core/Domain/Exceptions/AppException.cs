namespace Shared.Core.Domain.Exceptions;

public abstract class AppException : Exception
{
    public int Status { get; }
    public string Code { get; }
    public string Title { get; }

    public IDictionary<string, object?> Extensions { get; }


    protected AppException(
        string message,
        string code,
        int status,
        string title,
        IDictionary<string, object?>? extensions = null
    ) : base(message)
    {
        Code = code;
        Title = title;
        Status = status;
        Extensions = extensions ?? new Dictionary<string, object?>();
    }

    public AppException WithExtension(string key, object? value)
    {
        Extensions[key] = value;
        return this;
    }
}
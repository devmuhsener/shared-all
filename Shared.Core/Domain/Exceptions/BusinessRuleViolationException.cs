namespace Shared.Core.Domain.Exceptions;

public class BusinessRuleViolationException : AppException
{
    public BusinessRuleViolationException(
        string message,
        string code = "BUSINESS_RULE_VIOLATED",
        int status = 422,
        IDictionary<string, object?>? extensions = null
    ) : base(message, code, status, title: "Business Rule Violation", extensions: extensions)
    {
    }

    public BusinessRuleViolationException(
        string message,
        string ruleCode,
        IDictionary<string, object?>? extensions = null
    ) : base(
        message: message,
        code: ruleCode,
        status: 422,
        title: "Business Rule Violation",
        extensions: extensions
    )
    {
    }
}

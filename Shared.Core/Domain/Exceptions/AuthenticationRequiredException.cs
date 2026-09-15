namespace Shared.Core.Domain.Exceptions;

public class AuthenticationRequiredException : BaseDomainException
{
    public AuthenticationRequiredException() : base("Authentication required.")
    {
    }

    public AuthenticationRequiredException(string message) : base(message)
    {
    }
}
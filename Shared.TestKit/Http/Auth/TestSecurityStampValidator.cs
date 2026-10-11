using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Shared.TestKit.Http.Auth;

public class TestSecurityStampValidator : ISecurityStampValidator
{
    public Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        return Task.CompletedTask;
    }
}
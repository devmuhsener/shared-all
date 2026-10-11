using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.TestKit.Http.Auth;

public static class AuthCookieGenerator
{
    public static GeneratedCookie Generate(IServiceProvider sp, Action<AuthCookieBuilder>? configure = null)
    {
        var builder = new AuthCookieBuilder();
        configure?.Invoke(builder);

        var (ticket, scheme) = builder.BuildTicket();

        var dataProtectionProvider = sp.GetRequiredService<IDataProtectionProvider>();

        // Cookie middleware'inin şifre çözme zinciri
        var dataProtector = dataProtectionProvider.CreateProtector(
            "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
            scheme,
            "v2");

        var ticketFormat = new TicketDataFormat(dataProtector);
        var encryptedTicket = ticketFormat.Protect(ticket);


        var cookieName = scheme == IdentityConstants.ApplicationScheme
            ? ".AspNetCore.Identity.Application"
            : scheme == IdentityConstants.ExternalScheme
                ? "Identity.External"
                : scheme;

        return new GeneratedCookie(cookieName, encryptedTicket);
    }
}

public record GeneratedCookie(string Name, string Value)
{
    public string CookieHeaderValue => $"{Name}={Value}";
}
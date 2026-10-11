using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Shared.TestKit.Http.Auth;

public class AuthCookieBuilder
{
    private string _userId = Guid.NewGuid().ToString();
    private string _email = "testuser@example.com";
    private string _name = "Test User";
    private string? _securityStamp; // <-- EKLENDİ
    private string _scheme = CookieAuthenticationDefaults.AuthenticationScheme;
    private readonly List<string> _roles = ["USER"];
    private readonly List<Claim> _additionalClaims = [];

    // OAuth / External Login Metadata
    private bool _isExternal;
    private string? _provider;
    private string? _providerKey;

    public AuthCookieBuilder WithUserId(string userId)
    {
        _userId = userId;
        return this;
    }

    public AuthCookieBuilder WithUserId(Guid userId) => WithUserId(userId.ToString());

    public AuthCookieBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public AuthCookieBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public AuthCookieBuilder WithRole(string role)
    {
        _roles.Add(role);
        return this;
    }

    public AuthCookieBuilder WithRoles(params string[] roles)
    {
        _roles.AddRange(roles);
        return this;
    }

    public AuthCookieBuilder WithClaim(string type, string value)
    {
        _additionalClaims.Add(new Claim(type, value));
        return this;
    }

    public AuthCookieBuilder WithSecurityStamp(string securityStamp)
    {
        _securityStamp = securityStamp;
        return this;
    }

    // ==========================================
    // Scheme Konfigürasyonları
    // ==========================================

    /// <summary>
    /// Standart giriş yapmış uygulama kullanıcısı (Default Cookie)
    /// </summary>
    public AuthCookieBuilder AsDefaultScheme(string? customScheme = null)
    {
        _scheme = customScheme ?? IdentityConstants.ApplicationScheme;
        _isExternal = false;
        return this;
    }

    /// <summary>
    /// OAuth/Social Login Callback simülasyonu için External Scheme
    /// </summary>
    public AuthCookieBuilder AsExternalScheme(
        string provider = "GitHub",
        string? providerKey = null,
        string? customExternalScheme = null)
    {
        _scheme = customExternalScheme ?? IdentityConstants.ExternalScheme;
        _isExternal = true;
        _provider = provider;
        _providerKey = providerKey ?? _userId;
        return this;
    }

    internal (AuthenticationTicket Ticket, string Scheme) BuildTicket()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, _isExternal ? (_providerKey ?? _userId) : _userId),
            new(ClaimTypes.Name, _name),
            new(ClaimTypes.Email, _email),

            new("AspNet.Identity.SecurityStamp", _securityStamp ?? Guid.NewGuid().ToString()),
        };

        foreach (var role in _roles.Distinct())
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        claims.AddRange(_additionalClaims);

        var identity = new ClaimsIdentity(claims, _scheme);
        var principal = new ClaimsPrincipal(identity);

        var properties = new AuthenticationProperties();

        if (_isExternal)
        {
            properties.Items["LoginProvider"] = _provider ?? "External";
            properties.Items["xsrfId"] = _providerKey ?? _userId;
            properties.Items[".AuthScheme"] = _scheme;
        }

        return (new AuthenticationTicket(principal, properties, _scheme), _scheme);
    }
}
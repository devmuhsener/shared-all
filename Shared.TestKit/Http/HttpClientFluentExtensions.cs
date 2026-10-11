using System.Net.Http.Json;
using System.Text.Json;
using Bogus;
using Shared.Core.Context;
using Shared.TestKit.Http.Auth;
using Shared.TestKit.Http.RateLimiting;

namespace Shared.TestKit.Http;

public static class HttpClientFluentExtensions
{
    public static TestRequestBuilder Request(this HttpClient client, string uri, IServiceProvider serviceProvider) =>
        new(client, uri, serviceProvider);
}

public class TestRequestBuilder(HttpClient client, string uri, IServiceProvider serviceProvider)
{
    private static readonly Faker Faker = new();

    private string? _ip;
    private string? _cfCountry;
    private readonly Dictionary<string, string> _cookies = new();
    private readonly Dictionary<string, string> _headers = new();


    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public RateLimitAsserter ShouldExceedRateLimit(int permitLimit) => new(this, permitLimit);

    public TestRequestBuilder WithIp(string ip)
    {
        _ip = ip;
        return this;
    }

    public TestRequestBuilder WithCFCountry(string countryCode)
    {
        _cfCountry = countryCode;
        return this;
    }

    public TestRequestBuilder WithCookie(string name, string value)
    {
        _cookies[name] = value;
        return this;
    }

    public TestRequestBuilder WithCookie(string cookieHeaderValue)
    {
        var parts = cookieHeaderValue.Split('=', 2);
        if (parts.Length == 2)
        {
            _cookies[parts[0].Trim()] = parts[1].Trim();
        }

        return this;
    }

    public TestRequestBuilder WithCookie(GeneratedCookie cookie)
    {
        _cookies.Add(cookie.Name, cookie.Value);
        return this;
    }

    public TestRequestBuilder WithHeader(string name, string value)
    {
        _headers[name] = value;
        return this;
    }

    public TestRequestBuilder WithBearerToken(string token)
    {
        _headers["Authorization"] = $"Bearer {token}";
        return this;
    }
    
    

    


    public TestRequestBuilder WithAuthUser(Action<AuthCookieBuilder>? configure = null)
    {
        if (serviceProvider == null)
        {
            throw new InvalidOperationException(
                "ServiceProvider must be provided to Client.Request(uri, sp) to generate auth cookies.");
        }

        var cookie = AuthCookieGenerator.Generate(serviceProvider, builder =>
        {
            builder.AsDefaultScheme(); // varsayılan olarak normal authenticated user
            configure?.Invoke(builder);
        });

        return WithCookie(cookie);
    }

    public TestRequestBuilder WithExternalAuth(Action<AuthCookieBuilder>? configure = null)
    {
        if (serviceProvider == null)
        {
            throw new InvalidOperationException(
                "ServiceProvider must be provided to Client.Request(uri, sp) to generate auth cookies.");
        }

        var cookie = AuthCookieGenerator.Generate(serviceProvider, builder =>
        {
            builder.AsExternalScheme(); // varsayılan olarak OAuth external user
            configure?.Invoke(builder);
        });

        return WithCookie(cookie);
    }

    public async Task<HttpResponseMessage> GetAsync(CancellationToken ct = default)
    {
        var request = BuildRequest(HttpMethod.Get);
        return await client.SendAsync(request, ct);
    }

    public async Task<HttpResponseMessage> PostJsonAsync<T>(
        T data,
        JsonSerializerOptions? options = null,
        CancellationToken ct = default)
    {
        var request = BuildRequest(HttpMethod.Post);
        request.Content = JsonContent.Create(data, options: options ?? _options);
        return await client.SendAsync(request, ct);
    }

    public async Task<HttpResponseMessage> PutJsonAsync<T>(
        T data,
        JsonSerializerOptions? options = null,
        CancellationToken ct = default)
    {
        var request = BuildRequest(HttpMethod.Put);
        request.Content = JsonContent.Create(data, options: options ?? _options);
        return await client.SendAsync(request, ct);
    }

    public async Task<HttpResponseMessage> PatchJsonAsync<T>(
        T data,
        JsonSerializerOptions? options = null,
        CancellationToken ct = default)
    {
        var request = BuildRequest(HttpMethod.Patch);
        request.Content = JsonContent.Create(data, options: options ?? _options);
        return await client.SendAsync(request, ct);
    }

    public async Task<HttpResponseMessage> DeleteAsync(CancellationToken ct = default)
    {
        var request = BuildRequest(HttpMethod.Delete);
        return await client.SendAsync(request, ct);
    }

    private HttpRequestMessage BuildRequest(HttpMethod method)
    {
        var request = new HttpRequestMessage(method, uri);

        var clientIp = _ip ?? Faker.Internet.Ip();
        request.Headers.TryAddWithoutValidation("CF-Connecting-IP", clientIp);
        request.Headers.TryAddWithoutValidation("X-Forwarded-For", clientIp);

        if (!string.IsNullOrWhiteSpace(_cfCountry))
        {
            request.Headers.TryAddWithoutValidation("CF-IPCountry", _cfCountry);
        }

        if (_cookies.Count > 0)
        {
            var cookieHeader = string.Join("; ", _cookies.Select(c => $"{c.Key}={c.Value}"));
            request.Headers.TryAddWithoutValidation("Cookie", cookieHeader);
        }

        foreach (var (k, v) in _headers)
        {
            request.Headers.TryAddWithoutValidation(k, v);
        }

        return request;
    }
}
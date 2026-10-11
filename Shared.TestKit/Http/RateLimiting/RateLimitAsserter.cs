using System.Net;
using System.Text.Json;
using FluentAssertions;

namespace Shared.TestKit.Http.RateLimiting;

public class RateLimitAsserter(TestRequestBuilder builder, int permitLimit)
{
    public async Task<HttpResponseMessage> OnPostJsonAsync<T>(
        T payload,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default
    )
    {
        return await ExecuteLoopAsync(
            () => builder.PostJsonAsync(payload, options, cancellationToken),
            cancellationToken);
    }
    public async Task<HttpResponseMessage> OnPutJsonAsync<T>(T payload, JsonSerializerOptions? options = null , CancellationToken ct =default)
    {
       return await ExecuteLoopAsync(
           () => builder.PutJsonAsync(payload,options, ct),ct
           );
    }


    public async Task<HttpResponseMessage> OnGetAsync(CancellationToken ct = default)
    {
        return await ExecuteLoopAsync(
            () => builder.GetAsync(ct),
            ct);
    }

    private async Task<HttpResponseMessage> ExecuteLoopAsync(
        Func<Task<HttpResponseMessage>> sendRequest,
        CancellationToken ct)
    {
        for (var i = 1; i <= permitLimit; i++)
        {
            var allowedResponse = await sendRequest();

            allowedResponse.StatusCode.Should().NotBe(
                HttpStatusCode.TooManyRequests,
                because: $"request #{i} of {permitLimit} should be within the permitted rate limit window");
        }


        var blockedResponse = await sendRequest();

        blockedResponse.StatusCode.Should().Be(
            HttpStatusCode.TooManyRequests,
            because: $"request #{permitLimit + 1} must exceed the configured rate limit of {permitLimit}");


        return blockedResponse;
    }

  
}
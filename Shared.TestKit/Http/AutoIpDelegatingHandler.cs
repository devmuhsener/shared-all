using Bogus;

namespace Shared.TestKit.Http;

public class AutoIpDelegatingHandler : DelegatingHandler
{
    private static readonly Faker Faker = new Faker();

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!request.Headers.Contains("X-Forwarded-For") && !request.Headers.Contains("CF-Connecting-IP"))
        {
            var randomIp = Faker.Internet.Ip();
            request.Headers.TryAddWithoutValidation("X-Forwarded-For", randomIp);
            request.Headers.TryAddWithoutValidation("CF-Connecting-IP", randomIp);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
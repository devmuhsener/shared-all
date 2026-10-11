using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Shared.TestKit.Http;

public static class HttpResponseExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public static async Task<T> ReadAsJsonAsync<T>(
        this HttpResponseMessage response,
        JsonSerializerOptions? options = null)
    {
        var content = await response.Content.ReadFromJsonAsync<T>(options ?? JsonOptions);
        content.Should().NotBeNull("response body must contain a valid JSON payload");
        return content!;
    }


    public static async Task<CustomProblemDetails> ShouldBeValidationErrorAsync(
        this HttpResponseMessage response,
        string? expectedProperty = null,
        string? expectedErrorCode = null)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.ReadAsJsonAsync<CustomProblemDetails>();
        problem.Should().NotBeNull();

        var errors = problem.GetNormalizedErrors();
        errors.Should().NotBeNullOrEmpty("response payload must contain validation errors");

        if (!string.IsNullOrWhiteSpace(expectedProperty))
        {
            errors.Keys.Should().Contain(
                k => string.Equals(k, expectedProperty, StringComparison.OrdinalIgnoreCase),
                because: $"expected validation errors for property '{expectedProperty}'");

            if (!string.IsNullOrWhiteSpace(expectedErrorCode))
            {
                var errorsForProp = errors
                    .First(k => string.Equals(k.Key, expectedProperty, StringComparison.OrdinalIgnoreCase))
                    .Value;

                errorsForProp.Should().Contain(
                    expectedErrorCode,
                    because: $"property '{expectedProperty}' should contain error code '{expectedErrorCode}'");
            }
        }

        return problem;
    }

    // 2. Genel İş Kuralı / Domain Hatası Doğrulayıcı (Örn: PROFILE_ALREADY_EXISTS)
    public static async Task<CustomProblemDetails> ShouldBeAppExceptionAsync(
        this HttpResponseMessage response,
        HttpStatusCode expectedStatus,
        string expectedCode)
    {
        response.StatusCode.Should().Be(expectedStatus);

        var problem = await response.ReadAsJsonAsync<CustomProblemDetails>();
        problem.Should().NotBeNull();
        problem.Code.Should().Be(expectedCode);

        return problem;
    }

    /// <summary>
    /// Response içeriğini gezinilebilir JsonNode olarak döner.
    /// </summary>
    public static async Task<JsonNode> ReadAsJsonNodeAsync(this HttpResponseMessage response)
    {
        var rawJson = await response.Content.ReadAsStringAsync();
        rawJson.Should().NotBeNullOrWhiteSpace("response body should contain JSON");

        var node = JsonNode.Parse(rawJson);
        node.Should().NotBeNull("response body must parse into a valid JsonNode");

        return node!;
    }

    public static async Task<T> ReadPropertyAsync<T>(
        this HttpResponseMessage response,
        string propertyName)
    {
        var node = await response.ReadAsJsonNodeAsync();
        var prop = node[propertyName];

        prop.Should().NotBeNull($"expected property '{propertyName}' to exist in response JSON");
        return prop!.GetValue<T>();
    }

    /// <summary>
    /// Belirtilen property'nin beklenen değere eşit olduğunu tek adımda doğrular.
    /// </summary>
    public static async Task<HttpResponseMessage> ShouldHavePropertyAsync<T>(
        this HttpResponseMessage response,
        string propertyName,
        T expectedValue)
    {
        var actual = await response.ReadPropertyAsync<T>(propertyName);
        actual.Should().Be(
            expectedValue,
            because: $"property '{propertyName}' was expected to be {expectedValue}");

        return response;
    }


    public static async Task<CustomProblemDetails> ShouldBeNotFoundAsync(this HttpResponseMessage response,
        string? code = null)
    {
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var details = await response.ReadAsJsonAsync<CustomProblemDetails>();
        if (!string.IsNullOrWhiteSpace(code))
        {
            details.Code.Should().Be(code, $"'code' must be '{code}'");
        }

        return details;
    }

    public static async Task<CustomProblemDetails> ShouldBeUnauthorizedAsync(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        return await response.ReadAsJsonAsync<CustomProblemDetails>();
    }

    public static Task ShouldBeNoContent(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        return Task.CompletedTask;
    }

    public static async Task<CustomProblemDetails> ShouldBeConflictAsync(this HttpResponseMessage response,
        string? code = null)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var details = await response.ReadAsJsonAsync<CustomProblemDetails>();
        if (!string.IsNullOrWhiteSpace(code))
        {
            details.Code.Should().Be(code, $"'code' must be '{code}'");
        }

        return details;
    }


    public static async Task<CustomProblemDetails> ShouldBeForbiddenAsync(this HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        return await response.ReadAsJsonAsync<CustomProblemDetails>();
    }
}
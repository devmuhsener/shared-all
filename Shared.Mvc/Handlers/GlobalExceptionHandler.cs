using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Core.Domain.Exceptions;

namespace Shared.Mvc.Handlers
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogDebug(exception, "An error happened: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Instance = httpContext.Request.Path
            };

            switch (exception)
            {
                case EntityNotFoundException enf:
                    problemDetails.Title = "Entity not found.";
                    problemDetails.Status = (int)HttpStatusCode.NotFound;
                    problemDetails.Detail = enf.Message;
                    break;
                case IllegalDomainException illegal:
                    problemDetails.Title = "One or more validation errors occurred.";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Detail = illegal.Message;
                    break;
                case InvalidRequestException invalid:
                    problemDetails.Title = "One or more validation errors occurred.";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    problemDetails.Detail = invalid.Message;
                    break;
                case DuplicateEntityException duplicate:
                    problemDetails.Title = "Duplicate resource";
                    problemDetails.Status = (int)HttpStatusCode.Conflict;
                    problemDetails.Detail = duplicate.Message;
                    break;
                case AuthenticationRequiredException auth:
                    problemDetails.Title = "Authenticateion Required";
                    problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                    problemDetails.Detail = auth.Message;
                    break;
                case Shared.Core.Domain.Exceptions.UnauthorizedAccessException unauthorized:
                    problemDetails.Title = "Forbidden";
                    problemDetails.Status = (int)HttpStatusCode.Forbidden;
                    problemDetails.Detail = unauthorized.Message;
                    break;
                case ValidationException validation:
                {
                    var map = new Dictionary<string, string>();
                    problemDetails.Extensions.Add("errors",
                        validation.Errors.ToDictionary(x => x.PropertyName, x => x.ErrorMessage));
                }
                    problemDetails.Title = "One or more validation errors occurred.";
                    problemDetails.Status = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    logger.LogError(exception, "An error happened: {Message}", exception.Message);
                    problemDetails.Title = "Server error.";
                    problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                    problemDetails.Detail = "An unexpected server error happened.";
                    break;
            }


            httpContext.Response.StatusCode = problemDetails.Status.Value;
            httpContext.Response.ContentType = "application/problem+json";


            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
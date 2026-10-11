using Microsoft.AspNetCore.Builder;
using Shared.Mvc.Middlewares;

namespace Shared.Mvc.Extensions;

public static class ExecutionContextApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSharedExecutionContext(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExecutionContextMiddleware>();
    }
}
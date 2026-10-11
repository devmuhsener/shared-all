using MediatR;
using Shared.Core.Context;

namespace Shared.TestKit.MediatR;

public class MediatRTestHelper(IServiceProvider serviceProvider)
{
    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request,
        ExecutionContextData? contextData = null)
    {
        return await serviceProvider.ExecuteInScopeAsync<IMediator, TResponse>(
            async service => await service.Send(request),
            config => config.WithContextData(contextData)
        );
    }
}
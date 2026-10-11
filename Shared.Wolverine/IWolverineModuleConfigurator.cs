using Wolverine;

namespace Shared.Wolverine;

public interface IWolverineModuleConfigurator
{
    void Configure(WolverineOptions options);
}
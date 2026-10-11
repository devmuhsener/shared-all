using Microsoft.Extensions.Hosting;

namespace Shared.TestKit.Hosting;

public interface ITestFixture
{
    IServiceProvider Services { get; }

    IHost Host { get; }
    

}
using IOServices.Api.Input;
using IOServices.Api.Output;
using Processors.Api;

namespace Connector.Api;

public interface IConnector
{
    IInputService InputService { get; }
    IProcessor Processor { get; }
    IOutputService? OutputService { get; }
    Task StartReceive(CancellationToken token);
    void CheckHealth();
    
}
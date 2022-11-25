using PlatformEntities;

namespace IOServices.Api.Input;

public interface IInputService 
{
    event EventHandler<IInputServiceMessage> OnReceive;
    Task StartReceive(CancellationToken token);
    void CheckHealth(double secondsToResponse);
    void OnResponse(IInputServiceMessage input, IProcessorResult response);
}

public interface IInputService<TInputType> : IInputService
{
    event EventHandler<IInputServiceMessage<TInputType>> OnReceiveT;
    void OnResponse(IInputServiceMessage<TInputType> request, IProcessorResult response);
}
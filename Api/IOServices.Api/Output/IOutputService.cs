using IOServices.Api.Input;
using PlatformEntities;

namespace IOServices.Api.Output;

public interface IOutputService 
{
    OutputResponseModel Send(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult);
    Task<OutputResponseModel> SendAsync(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult, CancellationToken token);
    void CheckHealth(double secondsToResponse);
}